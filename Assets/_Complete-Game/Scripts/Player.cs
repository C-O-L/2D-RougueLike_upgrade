using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//玩家从MovingObject继承，我们的基类的对象可以移动，敌人也从这继承。
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//延迟时间(以秒为单位)以重新启动级别。
		public int pointsPerFood = 10;				//捡起一个食物对象时要加到玩家食物点数上的点数。
		public int pointsPerSoda = 20;				//捡起苏打水物品时要加到玩家食物点数上的点数。
		public int wallDamage = 1;					//玩家在砍墙时对墙壁造成的伤害。
		public Text foodText;						//显示当前玩家食物总数的UI文本。
		public AudioClip moveSound1;				//当玩家移动时播放的2个音频剪辑中的1个。
		public AudioClip moveSound2;				//2的2个音频剪辑播放时，玩家移动。
		public AudioClip eatSound1;					//当玩家收集食物对象时播放的2个音频片段中的1个。
		public AudioClip eatSound2;					//2的2个音频剪辑播放时，玩家收集食物对象。
		public AudioClip drinkSound1;				//当玩家收集苏打水对象时播放的2个音频剪辑中的1个。
		public AudioClip drinkSound2;				//2的2个音频剪辑播放时，玩家收集苏打对象。
		public AudioClip gameOverSound;				//音频剪辑播放时，玩家死亡。
		
		private Animator animator;					//用于存储对播放器的animator组件的引用。
		private int food;                           //用于在关卡中储存玩家的食物点数。
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//用于存储移动控件的屏幕触摸原点位置。
#endif
		
		
		//Start覆盖MovingObject的Start函数
		protected override void Start ()
		{
			//获取对播放器的animator组件的一个组件引用
			animator = GetComponent<Animator>();
			
			//在GameManager中获取当前食物点数总和。实例之间的水平。
			food = GameManager.instance.playerFoodPoints;
			
			//设置foodText来反映当前玩家的食物总数。
			foodText.text = "Food: " + food;
			
			//调用MovingObject基类的Start函数。
			base.Start ();
		}
		
		
		//当行为变为禁用或不活动时，将调用此函数。
		private void OnDisable ()
		{
			//当玩家对象被禁用时，将当前的本地食物总数存储在GameManager中，这样它就可以在下一层重新加载。
			GameManager.instance.playerFoodPoints = food;
		}
		
		
		private void Update ()
		{
			//如果不是轮到玩家，则退出该函数。
			if(!GameManager.instance.playersTurn) return;
			
			int horizontal = 0;  	//用于存储水平移动方向。
			int vertical = 0;		//用于存储垂直移动方向。
			
			//检查我们是否运行在Unity编辑器或在一个独立的构建。
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			//从输入管理器中获取输入，将其四舍五入为整数并以水平方式存储以设置x轴移动方向
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//从输入管理器获取输入，将其四舍五入为整数并垂直存储以设置y轴移动方向
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//检查是否水平移动，如果这样设置垂直为零。
			if(horizontal != 0)
			{
				vertical = 0;
			}
			//看看我们是在iOS、Android、Windows Phone 8还是Unity iPhone上运行
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//检查输入是否注册了超过零的触摸
			if (Input.touchCount > 0)
			{
				//存储检测到的第一个触摸。
				Touch myTouch = Input.touches[0];
				
				//检查触摸的相位是否等于开始
				if (myTouch.phase == TouchPhase.Began)
				{
					//如果是，将touchOrigin设置为该触摸的位置
					touchOrigin = myTouch.position;
				}
				
				//如果touch phase没有开始，而是等于end，并且touchOrigin的x大于或等于0:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//设置touchEnd等于这个触摸的位置
					Vector2 touchEnd = myTouch.position;
					
					//计算触点在x轴上的起始点和结束点之间的差值。
					float x = touchEnd.x - touchOrigin.x;
					
					//计算y轴上接触的开始点和结束点之间的差值。
					float y = touchEnd.y - touchOrigin.y;
					
					//设置touchOrigin。x到-1，这样我们的else if语句就会求值为false，而不会立即重复。
					touchOrigin.x = -1;
					
					//检查x轴上的差值是否大于y轴上的差值。
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//如果x大于0，将水平线设为1，否则设为-1
						horizontal = x > 0 ? 1 : -1;
					else
						//如果y大于0，将水平线设为1，否则设为-1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //移动平台依赖的编译部分的末尾以#elif开头
			//检查水平或垂直是否有非零值
			if(horizontal != 0 || vertical != 0)
			{
				//调用尝试移动传入通用参数墙，因为这是玩家可能与如果他们遇到一个(通过攻击它)
				//通过在水平和垂直作为参数指定方向的玩家移动。
				AttemptMove<Wall> (horizontal, vertical);
			}
		}
		
		//在基类MovingObject中，尝试移动覆盖了尝试移动函数
		//尝试移动采取一个通用参数T，这将为玩家的类型墙，它也为整数的x和y方向移动。
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//每次玩家移动时，从食物点数中减去总点数。
			food--;
			
			//更新食品文本显示，以反映当前得分。
			foodText.text = "Food: " + food;
			
			//调用基类的tmove方法，传入组件T(在本例中为Wall)和要移动的x和y方向。
			base.AttemptMove <T> (xDir, yDir);
			
			//Hit允许我们引用在Move中完成的Linecast的结果。
			RaycastHit2D hit;
			
			//如果Move返回true，这意味着玩家可以移动到一个空的空间。
			if (Move (xDir, yDir, out hit)) 
			{
				//调用SoundManager的RandomizeSfx播放移动声音，传入两个音频剪辑供选择。
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			}
			
			//因为玩家已经移动并且丢失了食物点数，检查游戏是否已经结束。
			CheckIfGameOver ();
			
			//设置GameManager的playersTurn布尔值为false，现在玩家已经结束。
			GameManager.instance.playersTurn = false;
		}
		
		
		//OnCantMove覆盖MovingObject中的抽象函数OnCantMove。
		//它接受一个通用参数T，在玩家的情况下，它是玩家可以攻击和摧毁的一堵墙。
		protected override void OnCantMove <T> (T component)
		{
			//设置hitWall等于作为参数传入的组件。
			Wall hitWall = component as Wall;
			
			//调用正在撞击的墙壁的DamageWall函数。
			hitWall.DamageWall (wallDamage);
			
			//设置玩家动画控制器的攻击触发，以播放玩家的攻击动画。
			animator.SetTrigger ("playerChop");
		}
		
		
		//当另一个对象进入附加到该对象的触发器碰撞器时，将发送OnTriggerEnter2D(仅适用于2D物理)。
		private void OnTriggerEnter2D (Collider2D other)
		{
			//检查触发器的标签是否与“退出”相碰撞。
			if(other.tag == "Exit")
			{
				//调用Restart函数以延迟restartLevelDelay(默认1秒)启动下一层。
				Invoke ("Restart", restartLevelDelay);
				
				//禁用播放器对象，因为级别已经结束。
				enabled = false;
			}
			
			//检查扳机上的标签是否与食物相撞。
			else if(other.tag == "Food")
			{
				//在玩家当前的食物总数中添加点sperfood。
				food += pointsPerFood;
				
				//更新foodText来表示当前的总数，并通知玩家他们获得了分数
				foodText.text = "+" + pointsPerFood + " Food: " + food;
				
				//调用SoundManager的RandomizeSfx函数，传入两个进食声音进行选择，以播放进食声音效果。
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
				//禁用玩家碰撞的食物对象。
				other.gameObject.SetActive (false);
			}
			
			//检查扳机上的标签是否与苏打水相撞。
			else if(other.tag == "Soda")
			{
				//将点数加到玩家的食物点数中
				food += pointsPerSoda;
				
				//更新foodText来表示当前的总数，并通知玩家他们获得了分数
				foodText.text = "+" + pointsPerSoda + " Food: " + food;
				
				//调用SoundManager的RandomizeSfx函数，传入两个饮酒声音进行选择，以播放饮酒声音效果。
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
				//禁用玩家碰撞的soda对象。
				other.gameObject.SetActive (false);
			}
		}
		
		
		//在调用时重新加载场景
		private void Restart ()
		{
			//加载最后的场景加载，在本例中主要是游戏中的唯一场景。我们加载它在“单一”模式，所以它取代了现有的一个
            //并且不加载当前场景中的所有场景对象。
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		//当敌人攻击玩家时，食物丢失将被调用。
		//它接受一个参数loss，该参数指定要丢失多少点。
		public void LoseFood (int loss)
		{
			//设置播放器动画器转换到playerHit动画的触发器。
			animator.SetTrigger ("playerHit");
			
			//从玩家的总得分中减去食物分。
			food -= loss;
			
			//用新的总数更新食品显示。
			foodText.text = "-"+ loss + " Food: " + food;
			
			//检查游戏是否已经结束。
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver检查玩家是否缺少食物点数，如果是，游戏结束。
		private void CheckIfGameOver ()
		{
			//检查食物总得分是否小于或等于零。
			if (food <= 0) 
			{
				//调用SoundManager的PlaySingle函数，并将gameoveround作为要播放的音频剪辑传递给它。
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//停止播放背景音乐。
				SoundManager.instance.musicSource.Stop();
				
				//调用GameManager的GameOver函数。
				GameManager.instance.GameOver ();
			}
		}
	}
}

