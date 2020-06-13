using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class Boss : MonoBehaviour
    {
        public float bulletSpeed;                                               //子弹速度
        public GameObject bulletPrefab;                                         //子弹
        Rigidbody2D rb;
        AudioSource au;
        public float fireRate;                                                  //发射频率
        float timer = 0f;                                                       //计时
        public int hp = 25;						                                // Enemy3的生命值。
        public GameObject[] PropTitle;                                          //一系列道具
        public GameObject[] prop;                                               //存储杀死boss后掉落的道具
        public int playerDamage; 							                    //玩家进行攻击时food数-1.
		

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();                                   //抓取Rigidbody2D组件
            au = GetComponent<AudioSource>();                                   //抓取AudioSource游戏组件，并保存在au里
			
        }

        private void Update() {
            timer += Time.deltaTime;
            // 每1.5秒发射一颗子弹
            if(timer > 1.5f){
                Fire();
                timer = 0f;
            }
        }
	
        //发射子弹的方法
        public void Fire()
        {
            
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            
            //定时销毁子弹
            Destroy(bullet, 10.0f);   
        }

        //掉血方法
        public void TakeDamage()
        {
            hp --;
            au.Play();                                                          //播放受伤音效
            if(hp == 0){
                Destroy(gameObject);                                                //销毁自己
                Produce();                                                          //调用Produce()方法生成道具
            }
        }

        // 掉落道具的方法
        void Produce()
        {
            Vector2 spawnPos = new Vector2(Random.Range(4.0f, 6.0f), 6.0f);          //在区间内生成道具
            
                //实例化道具
                prop[0] = Instantiate(PropTitle[0], spawnPos, Quaternion.identity);
                prop[1] = Instantiate(PropTitle[1], spawnPos, Quaternion.identity);
                prop[2] = Instantiate(PropTitle[2], spawnPos, Quaternion.identity);
                prop[3] = Instantiate(PropTitle[3], spawnPos, Quaternion.identity);
            
        }
    }

}