using UnityEngine;
using System.Collections;

namespace Completed
{
	//abstract关键字使您能够创建不完整的类和类成员，这些类和成员必须在派生类中实现。
	public abstract class MovingObject : MonoBehaviour
	{
		public float moveTime = 0.1f;			//物体移动所需的时间，以秒为单位。
		public LayerMask blockingLayer;			//层碰撞将被检查。
		
		
		private BoxCollider2D boxCollider; 		//BoxCollider2D组件附加到该对象。
		private Rigidbody2D rb2D;				//Rigidbody2D组件附加到这个对象。
		private float inverseMoveTime;			//用来使移动更有效。
		
		
		//受保护的虚函数可以通过继承类来重写。
		protected virtual void Start ()
		{
			//获取此对象的BoxCollider2D的组件引用
			boxCollider = GetComponent <BoxCollider2D> ();
			
			//获取此对象的Rigidbody2D的组件引用
			rb2D = GetComponent <Rigidbody2D> ();
			
			//通过存储移动时间的倒数我们可以用乘法而不是除法来使用它，这样更有效率。
			inverseMoveTime = 1f / moveTime;
		}
		
		
		//如果可以移动，则返回true;如果不能，则返回false。 
		//Move接受x方向、y方向和RaycastHit2D的参数来检查碰撞。
		protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
		{
			//存储开始位置移动，基于对象当前转换位置。
			Vector2 start = transform.position;
			
			// 根据调用Move时传入的方向参数计算端位置。
			Vector2 end = start + new Vector2 (xDir, yDir);
			
			//禁用boxCollider，这样linecast就不会撞上这个对象自己的collider。
			boxCollider.enabled = false;
			
			//在blockingLayer上从开始点到结束点进行换行检查碰撞。
			hit = Physics2D.Linecast (start, end, blockingLayer);
			
			//在linecast后重新启用boxCollider
			boxCollider.enabled = true;
			
			//检查是否有东西被击中
			if(hit.transform == null)
			{
				//如果没有命中，启动平滑移动协同例程，将Vector2端作为目标传入
				StartCoroutine (SmoothMovement (end));
				
				//移动成功返回true
				return true;
			}
			
			//如果什么东西被击中了，返回错误，移动是不成功的。
			return false;
		}
		
		
		//用于将单元从一个空间移动到下一个空间的协同例程使用参数end指定要移动到的位置。
		protected IEnumerator SmoothMovement (Vector3 end)
		{
			//用于将单元从一个空间移动到下一个空间的协同例程使用参数end指定要移动到的位置。根据当前位置与终端参数之差的平方值计算剩余移动距离。 
			//平方大小被用来代替大小因为它计算起来更便宜。
			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			//当这个距离大于一个非常小的量(几乎为零)时:
			while(sqrRemainingDistance > float.Epsilon)
			{
				//根据移动的时间比例找到一个更接近终点的新位置
				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				
				//调用附加刚体2d上的MovePosition并将其移动到计算的位置。
				rb2D.MovePosition (newPostion);
				
				//重新计算移动后的剩余距离。
				sqrRemainingDistance = (transform.position - end).sqrMagnitude;
				
				//返回并循环，直到sqrRemainingDistance足够接近于0来结束函数
				yield return null;
			}
		}
		
		
		//virtual关键字意味着可以通过使用override关键字继承类来重写尝试移动。
		//尝试移动采用一个通用参数T来指定我们希望我们的单位在被阻挡时与之交互的组件类型(玩家是敌人，墙壁是玩家)。
		protected virtual void AttemptMove <T> (int xDir, int yDir)
			where T : Component
		{
			//当调用Move时，Hit将存储我们的linecast命中的所有内容。
			RaycastHit2D hit;
			
			//如果移动成功，设置可以移动为真;如果失败，设置为假。
			bool canMove = Move (xDir, yDir, out hit);
			
			//检查是否没有被linecast击中
			if(hit.transform == null)
				//如果没有命中任何目标，则返回并不再执行进一步的代码。
				return;
			
			//获取一个对类型为T的组件的引用，该组件附加到被命中的对象上
			T hitComponent = hit.transform.GetComponent <T> ();
			
			//如果canMove为false且hitComponent不等于null，则意味着MovingObject被阻塞，并命中了它可以与之交互的对象。
			if(!canMove && hitComponent != null)
				
				//调用OnCantMove函数并将其作为参数传递给hitComponent。
				OnCantMove (hitComponent);
		}
		
		
		//抽象修饰符表示被修改的对象缺少或不完整的实现。
		//OnCantMove将被继承类中的函数覆盖。
		protected abstract void OnCantMove <T> (T component)
			where T : Component;
	}
}
