using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class Enemy3_bullet : MonoBehaviour
    {
        public float bulletSpeed;                                               //子弹速度
        public GameObject bulletPrefab;                                         //子弹
        public Transform firePoint;                                             //发炮位置
        Rigidbody2D rb;
        public float fireRate;                                                  //发射频率
        float timer = 0f;                                                       //计时
        public int hp = 2;						                                // Enemy3的生命值。
        public GameObject[] PropTitle;                                          //一系列道具
        public GameObject prop;
        private bool skipMove;								                    //用布尔值决定敌人在这一回合进行移动还是跳过.
        public int playerDamage; 							                    //玩家进行攻击时food数-1.
		public AudioClip attackSound1;						                    //攻击音频1.
		public AudioClip attackSound2;						                    //攻击音频2.
		
		
		private Animator animator;							                    //动画组件.
		private Transform target;							                    //转换目标.

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();                                   //抓取Rigidbody2D组件
			
        }

        private void Update() {
            timer += Time.deltaTime;
            if(timer > 0.8f){
                Fire();
                timer = 0f;
            }
        }
	
        //发射子弹的方法
        public void Fire()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            //抓取子弹的Rigidbody2D组件，速度方向设置为下，大小设置为bulletSpeed
            // bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * bulletSpeed;
            //定时销毁子弹
            Destroy(bullet, 10.0f);   
        }

        // //子弹碰撞
        // private void OnTriggerEnter2D(Collider2D collision) {
        //     //如果碰到玩家
        //     if(collision.gameObject != null)
        //     {
        //         Debug.Log(collision.gameObject.name);
        //         Destroy(collision.gameObject);                                  //销毁子弹
        //         FindObjectOfType<Player>().LoseFood(30);
        //     }
        // }
        
        // 敌人碰撞方法
        private void OnCollisionEnter2D(Collision2D collision) {
            // 如果碰到子弹
            if(collision.gameObject.CompareTag("Bullet"))
            {
                Destroy(collision.gameObject);                                  //销毁子弹
                DamageEnemy ();                      
            }
        }

        //掉血方法
        public void TakeDamage()
        {
            hp --;
            if(hp == 0){
                Destroy(gameObject);                                                //销毁自己
                GameManager.Instance.playersTurn = true;
                Produce();                                                          //调用Produce()方法生成道具
            }
        }

        // 掉落道具的方法
        void Produce()
        {
            Vector2 spawnPos = new Vector2(Random.Range(2f, 6f), 6f);          //在区间内生成道具
            int num = Random.Range(1, 6);                                      //生成随机数范围1-5
            if(num == 1){
                //实例化PropTitle[]道具数组中的第一个
                prop = Instantiate(PropTitle[0], spawnPos, Quaternion.identity);
            }
            if(num == 2){
                //实例化PropTitle[]道具数组中的第二个
                prop = Instantiate(PropTitle[1], spawnPos, Quaternion.identity);
            }
            if(num == 3){
                //实例化PropTitle[]道具数组中的第san个
                prop = Instantiate(PropTitle[2], spawnPos, Quaternion.identity);
            }
            if(num == 4){
                //实例化PropTitle[]道具数组中的第si个
                prop = Instantiate(PropTitle[3], spawnPos, Quaternion.identity);
            }
        }


        //当玩家攻击Enemy3时，将会调用DamageEnemy。
        public void DamageEnemy ()
        {
            //从生命值中减去损失。
            hp --;
            if(hp == 0){
                TakeDamage();
            }
        }
    }
}