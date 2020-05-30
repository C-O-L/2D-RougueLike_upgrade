using UnityEngine;
using System.Collections;

namespace Completed
{
	public class SoundManager : MonoBehaviour 
	{
		public AudioSource efxSource;                   //拖动对将播放声音效果的音频源的引用。
		public AudioSource musicSource;                 //拖动对将播放音乐的音频源的引用。
		public static SoundManager instance = null;     //允许其他脚本从SoundManager调用函数。				
		public float lowPitchRange = .95f;              //最低的a音效将随机调高。
		public float highPitchRange = 1.05f;            //最高的a音效将随机调高。


		void Awake ()
		{
			//检查是否已存在SoundManager实例
			if (instance == null)
				//如果没有，请设置为这个。
				instance = this;
			//如果实例已存在：
			else if (instance != this)
				//销毁它，这将强制执行我们的单例模式，因此只能有一个SoundManager实例。
				Destroy(gameObject);

			//将SoundManager设置为DontDestroyOnLoad，以便在重新加载场景时不会破坏它。
			DontDestroyOnLoad(gameObject);
		}


		//用于播放单个声音片段。
		public void PlaySingle(AudioClip clip)
		{
			//将efxSource音频源的剪辑设置为作为参数传入的剪辑。
			efxSource.clip = clip;

			//播放剪辑。
			efxSource.Play ();
		}


		//RandomizeSfx在不同的音频剪辑之间随机选择，并稍微改变它们的音调。
		public void RandomizeSfx (params AudioClip[] clips)
		{
			//RandomizeSfx在不同的音频剪辑之间随机选择，并稍微改变它们的音调。
			int randomIndex = Random.Range(0, clips.Length);

			//选择一个随机音高，在高低音高之间播放剪辑。
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);

			//将音频源的音调设置为随机选择的音调。
			efxSource.pitch = randomPitch;

			//将剪辑设置为我们随机选择的索引处的剪辑。
			efxSource.clip = clips[randomIndex];

			//播放剪辑。
			efxSource.Play();
		}
	}
}
