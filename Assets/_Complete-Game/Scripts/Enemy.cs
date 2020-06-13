using UnityEngine;
using System.Collections;

namespace Completed
{
	//敌人从MovingObject继承，我们的基类的对象可以移动，玩家也从这继承。
	public class Enemy : MovingObject
	{
		public int playerDamage; 							//玩家进行攻击时food数-1.
		public AudioClip attackSound1;						//攻击音频1.
		public AudioClip attackSound2;						//攻击音频2.
		
		private Animator animator;							//动画组件.
		private Transform target;							//转换目标.
		private bool skipMove;								//用布尔值决定敌人在这一回合进行移动还是跳过.

		
		//重写基类的虚Start函数。
		protected override void Start ()
		{
			//通过把这个敌人添加到敌人类中，向GameManager实例注册这个敌人. 
			//允许GameManager发出移动指令.
			GameManager.instance.AddEnemyToList (this);
			
			//获取并存储对动画组件的引用.
			animator = GetComponent<Animator> ();

			//使用Player GameObject的标记找到它，并存储对其transform组件的引用.
			target = GameObject.FindGameObjectWithTag ("Player").transform;

			//调用基类MovingObject的start函数.
			base.Start ();
		}


		//重写MovingObject的AttemptMove功能，以包含敌人跳过回合所需的功能.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//检查skipMove是否为true，如果为false，则跳过此回合.
			if (skipMove)
			{
				skipMove = false;
				return;
				
			}

			//从MovingObject调用AttemptMove函数.
			base.AttemptMove <T> (xDir, yDir);

			//现在敌人已经移动，设置skip move为true以跳过下一步.
			skipMove = true;
		}


		//每个回合GameManger会告诉每个敌人试图向玩家移动.
		public void MoveEnemy ()
		{
			//声明X轴和Y轴移动方向的变量，范围从-1到1.
			//这些值允许我们在基本方向之间进行选择：向上、向下、向左和向右.
			int xDir = 0;
			int yDir = 0;

			//如果位置差约为零（Epsilon），请执行以下操作：
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)

				//如果目标（玩家）位置的y坐标大于敌人位置的y坐标，则设置y方向1（向上移动）。如果没有，则将其设置为-1（向下移动）。
				yDir = target.position.y > transform.position.y ? 1 : -1;

			//如果位置差约为零（Epsilon），请执行以下操作：
			else
				//检查目标x位置是否大于敌人的x位置，如果大于，则将x方向设置为1（向右移动），如果未设置为-1（向左移动）。
				xDir = target.position.x > transform.position.x ? 1 : -1;

			//调用AttemptMove函数并传入泛型参数Player，因为敌人正在移动，并且可能会遇到Player
			AttemptMove<Player> (xDir, yDir);
		}

		//OnCantMove被调用如果敌人试图移动到玩家占据的空间，它会覆盖MovingObject的OnCantMove函数并接受一个通用参数T，
		// 我们用它来传递我们希望遇到的组件，在这种情况下，玩家
		protected override void OnCantMove <T> (T component)
		{
			//声明hitPlayer并将其设置为与遇到的组件相等.
			Player hitPlayer = component as Player;

			//调用hitPlayer的LoseFood函数传递playerDamage，减去foodpoints的数量。
			hitPlayer.LoseFood (playerDamage);

			//设置animator的攻击触发器以触发敌人攻击动画.
			animator.SetTrigger ("enemyAttack");

			//调用SoundManager传入两个音频剪辑的RandomizeSfx函数，在两者之间随机选择。
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}

	}
}
