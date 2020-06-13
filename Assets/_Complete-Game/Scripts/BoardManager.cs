using UnityEngine;
using System;
using System.Collections.Generic; 		//允许我们使用列表。
using Random = UnityEngine.Random; 		//告诉 Random 使用单位引擎随机数生成器。

namespace Completed
	
{
	
	public class BoardManager : MonoBehaviour
	{
		// 使用Serializable允许我们在检查器中嵌入带有子属性的类。
		[Serializable]
		public class Count
		{
			public int minimum; 			//我们的Count类的最小值。
			public int maximum; 			//我们的Count类的最大值。
			
			
			//赋值构造函数。
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int columns = 8; 										//游戏板中的列数。
		public int rows = 8;											//游戏板中的行数。
		public Count wallCount = new Count (5, 9);						//每层随机墙数的上限和下限。
		public Count foodCount = new Count (1, 5);						//每层随机食物数量的上限和下限。
		public Count propCount = new Count (0, 1);                      //每层随机道具数量的上限和下限。
		public Count bulletCount = new Count (0, 5);                    //每层随机弹药数量的上限和下限。
		public Count enemy3Count = new Count (2, 5);                    //每层随机会发射子弹的敌人数量的上限和下限。
		public Count foodForBulletCount = new Count (1, 1);             //5的倍数关卡出现的食物换取弹药npc的上下限，即出现一个
		public GameObject exit;											//预置出口。
		public GameObject[] floorTiles;									//地板预制件阵列。
		public GameObject[] wallTiles;									//墙预制件阵列。
		public GameObject[] foodTiles;									//一系列的预制食品。
		public GameObject[] propTiles;                                  //一系列的预制道具。
		public GameObject[] bulletTiles;                                //一系列的预制弹药。
		public GameObject[] enemyTiles;									//敌人预制件阵列。
		public GameObject[] enemy3Tiles;								//敌人3预制件阵列。
		public GameObject boss;                                         //Boss
		public GameObject[] outerWallTiles;								//外瓦预制件阵列。
		public GameObject[] foodForBullet;                              //食物换取弹药，只在5的倍数关出现
		
		private Transform boardHolder;									//一个变量，用于存储对Board对象转换的引用。
		private List <Vector3> gridPositions = new List <Vector3> ();	//放置磁贴的可能位置列表。
		
		
		//清除我们的列表网格位置并准备生成一个新板。
		public void InitialiseList ()
		{
			//清除我们的列表网格位置。
			gridPositions.Clear ();
			
			//循环通过x轴(列)。
			for(int x = 1; x < columns-1; x++)
			{
				//在每一列中，循环通过y轴(行)。
				for(int y = 1; y < rows-1; y++)
				{
					//在每个索引处用该位置的x和y坐标向列表添加一个新的Vector3。
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		
		//设置游戏板的外墙和地板(背景)。
		void BoardSetup ()
		{
			//实例化Board并将其转换设置为boardHolder。
			boardHolder = new GameObject ("Board").transform;
			
			//沿着x轴进行循环，从-1开始(以填充角)使用地板或外壁边缘瓷砖。
			for(int x = -1; x < columns + 1; x++)
			{
				//沿y轴循环，从-1开始放置地板或外墙瓷砖。
				for(int y = -1; y < rows + 1; y++)
				{
					//从我们的地砖预制件数组中随机选择一个瓦片，并准备实例化它。
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
					//检查我们当前的位置是否在板边，如果是这样，从我们的外墙瓷砖数组中随机选择一个外墙预制件。
					if(x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					//使用prefab实例化GameObject实例，该prefab选择用于在循环中当前网格位置对应的Vector3处实例化，并将其转换为GameObject。
					GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//将我们新实例化的对象实例的父级设置为boardHolder，这只是组织性的，以避免混乱的层次结构。
					instance.transform.SetParent (boardHolder);
				}
			}
		}
		
		
		//RandomPosition从我们的列表gridPositions返回一个随机位置。
		Vector3 RandomPosition ()
		{
			//声明一个整数randomIndex，将其值设置为一个随机数，该随机数介于0和我们的List gridPositions中的项数之间。
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//声明一个名为randomPosition的变量类型为Vector3，将其值从我们的List gridPositions设置为条目at randomIndex。
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//从列表中删除randomIndex条目，这样它就不能被重用了。
			gridPositions.RemoveAt (randomIndex);
			
			//返回随机选择的Vector3位置。
			return randomPosition;
		}
		
		
		//LayoutObjectAtRandom接受一个游戏对象数组来进行选择，以及创建对象数量的最小和最大范围。
		public void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//选择要在最小和最大限制内实例化的随机对象数
			int objectCount = Random.Range (minimum, maximum+1);
			
			//实例化对象，直到达到随机选择的限制objectCount为止
			for(int i = 0; i < objectCount; i++)
			{
				//通过从gridPosition中存储的可用vector3列表中获取一个随机位置，为randomPosition选择一个位置
				Vector3 randomPosition = RandomPosition();
				
				//从tileArray中随机选择一个游戏对象，并将其赋给tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//实例化tileChoice在不改变旋转的随机位置返回的位置
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}
		
		
		//SetupScene初始化我们的关卡并调用前面的函数来布局游戏板
		public void SetupScene (int level)
		{
			//创建外墙和地板。
			BoardSetup ();
			
			//重置我们的网格位置列表。
			InitialiseList ();
			
			//根据最小值和最大值，在随机位置实例化随机数量的墙砖。
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			
			//在随机位置实例化基于最小值和最大值的随机数量的食物块。
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

			//在随机位置实例化基于最小值和最大值的随机数量的道具。
			LayoutObjectAtRandom (propTiles, propCount.minimum, propCount.maximum);

			//在随机位置实例化基于最小值和最大值的随机数量的弹药。
			LayoutObjectAtRandom (bulletTiles, bulletCount.minimum, bulletCount.maximum);
			
			//根据当前等级的数量，根据对数级数确定敌人的数量
			int enemyCount = (int)Mathf.Log(level, 2f);
			
			//基于最小值和最大值，在随机位置实例化一个随机数量的敌人。
			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);

			//基于最小值和最大值，在随机位置实例化一个随机数量的敌人3。
			LayoutObjectAtRandom (enemy3Tiles, enemy3Count.minimum, enemy3Count.maximum);
			
			//实例化游戏板右上角的出口平铺
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
			
			
			// 如果关卡数为5的倍数，出现食物换取道具的Prop
			if(level % 5 == 0){
				LayoutObjectAtRandom (foodForBullet, foodForBulletCount.minimum, foodForBulletCount.maximum);
				// GameObject a = Instantiate(foodForBullet[0], new Vector3(Random.Range(2, 2), Random.Range(6, 6), 0), Quaternion.identity);
			}

			// 如果关卡数为10的倍数，出现Boss
			if(level % 10 == 0){
				// GameObject a = Instantiate(boss[0], new Vector3(Random.Range(8, 5), Random.Range(8.5f, 5), 0), Quaternion.identity);
				// 在出口位置实例化一个敌人，敌人没死之前不能通关
				Instantiate (boss, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
			}
		}
	}
}
