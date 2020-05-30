using UnityEngine;
using System.Collections;

namespace Completed
{
	//敌人从MovingObject继承，我们的基类的对象可以移动，玩家也从这继承。
	public class Enemy : MovingObject
	{
		public int playerDamage; 							//玩家进行攻击时food数-1 The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//攻击音频1 First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//攻击音频2 Second of two audio clips to play when attacking the player.
		
		
		private Animator animator;							//动画组件 Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target;							//转换目标 Transform to attempt to move toward each turn.
		private bool skipMove;								//用布尔值决定敌人在这一回合进行移动还是跳过 Boolean to determine whether or not enemy should skip a turn or move this turn.
		
		
		//重写基类的虚Start函数。
		protected override void Start ()
		{
			//通过把这个敌人添加到敌人类中，向GameManager实例注册这个敌人 Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//允许GameManager发出移动指令 This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList (this);
			
			//获取并存储对动画组件的引用 Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();

			//使用Player GameObject的标记找到它，并存储对其transform组件的引用  Find the Player GameObject using it's tag and store a reference to its transform component.
			target = GameObject.FindGameObjectWithTag ("Player").transform;

			//调用基类MovingObject的start函数。 Call the start function of our base class MovingObject.
			base.Start ();
		}


		//重写MovingObject的AttemptMove功能，以包含敌人跳过回合所需的功能。Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//检查skipMove是否为true，如果为false，则跳过此回合  Check if skipMove is true, if so set it to false and skip this turn.
			if (skipMove)
			{
				skipMove = false;
				return;
				
			}

			//从MovingObject调用AttemptMove函数  Call the AttemptMove function from MovingObject.
			base.AttemptMove <T> (xDir, yDir);

			//现在敌人已经移动，设置skip move为true以跳过下一步  Now that Enemy has moved, set skipMove to true to skip next move.
			skipMove = true;
		}


		//每个回合GameManger会告诉每个敌人试图向玩家移动  MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
		public void MoveEnemy ()
		{
			//声明X轴和Y轴移动方向的变量，范围从-1到1  Declare variables for X and Y axis move directions, these range from -1 to 1.
			//这些值允许我们在基本方向之间进行选择：向上、向下、向左和向右 These values allow us to choose between the cardinal directions: up, down, left and right.
			int xDir = 0;
			int yDir = 0;

			//如果位置差约为零（Epsilon），请执行以下操作：If the difference in positions is approximately zero (Epsilon) do the following:
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)

				//如果目标（玩家）位置的y坐标大于敌人位置的y坐标，则设置y方向1（向上移动）。如果没有，则将其设置为-1（向下移动）。
				//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
				yDir = target.position.y > transform.position.y ? 1 : -1;

			//如果位置差约为零（Epsilon），请执行以下操作： If the difference in positions is not approximately zero (Epsilon) do the following:
			else
				//检查目标x位置是否大于敌人的x位置，如果大于，则将x方向设置为1（向右移动），如果未设置为-1（向左移动）。
				//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
				xDir = target.position.x > transform.position.x ? 1 : -1;

			//调用AttemptMove函数并传入泛型参数Player，因为敌人正在移动，并且可能会遇到Player
			//Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
			AttemptMove<Player> (xDir, yDir);
		}

		//OnCantMove被调用如果敌人试图移动到玩家占据的空间，它会覆盖MovingObject的OnCantMove函数并接受一个通用参数T，我们用它来传递我们希望遇到的组件，在这种情况下，玩家
		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected override void OnCantMove <T> (T component)
		{
			//声明hitPlayer并将其设置为与遇到的组件相等 Declare hitPlayer and set it to equal the encountered component.
			Player hitPlayer = component as Player;

			//调用hitPlayer的LoseFood函数传递playerDamage，减去foodpoints的数量。 Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
			hitPlayer.LoseFood (playerDamage);

			//设置animator的攻击触发器以触发敌人攻击动画 Set the attack trigger of animator to trigger Enemy attack animation.
			animator.SetTrigger ("enemyAttack");

			//调用SoundManager传入两个音频剪辑的RandomizeSfx函数，在两者之间随机选择。 Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}
	}
}
