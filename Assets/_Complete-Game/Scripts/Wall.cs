using UnityEngine;
using System;
using System.Collections.Generic; 		//允许我们使用列表。
using Random = UnityEngine.Random; 		//告诉 Random 使用单位引擎随机数生成器。


namespace Completed
{
	public class Wall : MonoBehaviour
	{
		public AudioClip chopSound1;				//当玩家攻击墙壁时播放的2个音频片段中的1个。
		public AudioClip chopSound2;				//当玩家攻击墙壁时播放的2个音频片段中的2个。
		public Sprite dmgSprite;					//在墙被玩家攻击后显示另一个精灵。
		public int hp = 3;							//墙的生命值。
		public GameObject[] propTiles;              //一系列预制道具。
		// public Vector3 a = Vector3.one;
		// public Quaternion b = new Quaternion(0,0,0,0);
		public Vector2 minPos,maxPos;               //生成道具的位置范围
		
		private SpriteRenderer spriteRenderer;		//将组件引用存储到附加的SpriteRenderer。
		
		
		void Awake ()
		{
			//获取对SpriteRenderer的组件引用。
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}
		
		
		//当玩家攻击一堵墙时，将会调用DamageWall。
		public void DamageWall (int loss)
		{
			//调用SoundManager的RandomizeSfx函数来播放两个chop声音中的一个。
			SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			
			//将spriteRenderer设置为损坏的墙壁精灵。
			spriteRenderer.sprite = dmgSprite;
			
			//从生命值中减去损失。
			hp -= loss;
			
			//如果生命值小于或等于零:掉落道具（作业2）
			if(hp <= 0){
				//禁用gameObject。
				gameObject.SetActive (false);

				// Generate();
				//实例化道具
				// GameObject go = GameObject.Instantiate(propTiles[0], a, b);
				// GameObject to = GameObject.Instantiate(propTiles[1], a, b);
                
				GameObject a = Instantiate(propTiles[0], new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0), Quaternion.identity);
    			GameObject b = Instantiate(propTiles[1], new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0), Quaternion.identity);
			}
		}

		// public void Generate()
		// {
    	// 	GameObject a = Instantiate(propTiles[0], new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0), Quaternion.identity);
    	// 	GameObject b = Instantiate(propTiles[1], new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0), Quaternion.identity);
    	// 	a.transform.SetParent(this.transform);b.transform.SetParent(this.transform);
    	// 	if(Vector3.Distance(a.transform.position,b.transform.position) < 4f)
    	// 	{
        // 		Destroy(a);Destroy(b);
        // 		Generate();
    	// 	}
		// }
	}
}
