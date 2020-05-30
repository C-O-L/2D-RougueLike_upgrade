using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//允许我们使用列表。 
	using UnityEngine.UI;					//允许我们使用UI。
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//在开始关卡前等待的时间，以秒为单位。
		public float turnDelay = 0.1f;							//每个玩家回合之间的延迟。
		public int playerFoodPoints = 100;						//玩家食物点数的起始值。
		public static GameManager instance = null;				//GameManager的静态实例，允许任何其他脚本访问它。
		[HideInInspector] public bool playersTurn = true;		//布尔值检查如果它是玩家转身，隐藏在检查，但公共。
		
		
		private Text levelText;									//显示当前级别号的文本。
		private GameObject levelImage;							//在设置关卡的时候，图片要屏蔽关卡，背景为levelText。
		private BoardManager boardScript;						//存储对我们的BoardManager的引用，它将设置该级别。
		private int level = 1;									//当前等级编号，在游戏中表示为“第一天”。
		private List<Enemy> enemies;							//所有敌人单位的列表，用来发布他们的移动命令。
		private bool enemiesMoving;								//布尔值检查敌人是否移动。
		private bool doingSetup = true;							//布尔值检查我们是否正在设置板，防止玩家在设置过程中移动。
		
		
		
		//Awake总是在任何Start函数之前调用
		void Awake()
		{
            //检查实例是否已经存在
            if (instance == null)

                //如果不是，则将其设置为instance
                instance = this;

            //如果实例已经存在，而它不是这个:
            else if (instance != this)

                //然后摧毁。这加强了我们的单例模式，这意味着GameManager只能有一个实例。
                Destroy(gameObject);	
			
			//将此设置为在重新加载场景时不被销毁
			DontDestroyOnLoad(gameObject);
			
			//将敌人分配到一个新的敌人列表中。
			enemies = new List<Enemy>();
			
			//获取对附加的BoardManager脚本的组件引用
			boardScript = GetComponent<BoardManager>();
			
			//调用InitGame函数初始化第一层
			InitGame();
		}

        //这只被调用一次，并且参数告诉它只在场景加载后才被调用
        //(否则，我们的场景加载回调会被称为第一次加载，我们不希望这样)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //注册在每次加载场景时调用的回调函数
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //每次加载一个场景时都会调用这个函数。
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;
            instance.InitGame();
        }

		
		//初始化每个关卡的游戏。
		void InitGame()
		{
			//当doingSetup设置为true时，玩家不能移动，阻止玩家在纸牌打开时移动。
			doingSetup = true;
			
			//通过名称找到我们的image LevelImage的引用。
			levelImage = GameObject.Find("LevelImage");
			
			//通过按名称查找并调用GetComponent来获得对text LevelText的文本组件的引用。
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			
			//将levelText的文本设置为字符串“Day”并追加当前的级别号。
			levelText.text = "Day " + level;
			
			//设置levelImage在设置过程中活动阻止玩家查看游戏板。
			levelImage.SetActive(true);
			
			//使用以秒为单位的levelStartDelay来调用HideLevelImage函数。
			Invoke("HideLevelImage", levelStartDelay);
			
			//清除列表中的所有敌人，为下一关做准备。
			enemies.Clear();
			
			//调用BoardManager脚本的SetupScene函数，传递当前级别号。
			boardScript.SetupScene(level);
			
		}
		
		
		//隐藏在层之间使用的黑色图像
		void HideLevelImage()
		{
			//禁用levelImage gameObject。
			levelImage.SetActive(false);
			
			//将doingSetup设置为false，允许玩家再次移动。
			doingSetup = false;
		}
		
		//每一帧都调用Update。
		void Update()
		{
			//检查playersTurn或enemiesMoving或doingSetup当前是否为真。
			if(playersTurn || enemiesMoving || doingSetup)
				
				//如果这些都是正确的，返回并且不要开始移动敌人。
				return;
			
			//开始移动的敌人。
			StartCoroutine (MoveEnemies ());
		}
		
		//调用此函数可将传入的敌方对象添加到敌方对象列表中。
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}
		
		
		//当玩家到达0食物点时游戏结束
		public void GameOver()
		{
			//设置关卡文本来显示关卡传递的数量和游戏传递消息
			levelText.text = "After " + level + " days, you starved.";
			
			//启用黑色背景图像游戏对象。
			levelImage.SetActive(true);
			
			//这个GameManager禁用。
			enabled = false;

			// //调用Gamequit
			// Gamequit();
		}
		
		//协同程序按顺序移动敌人。
		IEnumerator MoveEnemies()
		{
			//当敌人移动时，玩家不能移动。
			enemiesMoving = true;
			
			//等待turnDelay秒，默认为.1(100毫秒)。
			yield return new WaitForSeconds(turnDelay);
			
			//如果没有敌人滋生(即在第一级):
			if (enemies.Count == 0) 
			{
				//在移动之间等待turnDelay秒，代替没有敌人移动时造成的延迟。
				yield return new WaitForSeconds(turnDelay);
			}
			
			//循环遍历敌对对象列表。
			for (int i = 0; i < enemies.Count; i++)
			{
				//在敌人列表的索引i处调用敌人的MoveEnemy函数。
				enemies[i].MoveEnemy ();
				
				//在移动下一个敌人之前，等待敌人的移动时间， 
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			//一旦敌人移动完毕，将playersTurn设置为true，这样玩家就可以移动了。
			playersTurn = true;
			
			//敌人移动完毕，设置敌人移动为false。
			enemiesMoving = false;
		}
	}
}

