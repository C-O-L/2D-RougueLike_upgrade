using UnityEngine;
using System.Collections;

namespace Completed
{	
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;			//GameManager预置实例化。
		public GameObject soundManager;			//SoundManager预置实例化。
		
		
		void Awake ()
		{
			//检查GameManager是否已经被分配给静态变量GameManager。如果它仍然是空的
			if (GameManager.instance == null)
				
				//实例化gameManager预制
				Instantiate(gameManager);
			
			//检查SoundManager是否已经被分配给静态变量GameManager。如果它仍然是空的
			if (SoundManager.instance == null)
				
				//实例化SoundManager预制
				Instantiate(soundManager);
		}
	}
}