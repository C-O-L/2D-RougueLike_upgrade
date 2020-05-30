using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Wall : MonoBehaviour
	{
		public AudioClip chopSound1;				//当玩家攻击墙壁时播放的2个音频片段中的1个。
		public AudioClip chopSound2;				//当玩家攻击墙壁时播放的2个音频片段中的2个。
		public Sprite dmgSprite;					//在墙被玩家攻击后显示另一个精灵。
		public int hp = 3;							//墙的生命值。
		
		
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
			if(hp <= 0)
				//禁用gameObject。
				gameObject.SetActive (false);
		}
	}
}
