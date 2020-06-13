using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{

    public class BulletPlayer : MonoBehaviour
    {
         public float bulletSpeed = 0.01f;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(transform.up*bulletSpeed*Time.deltaTime, Space.World);
        }

        //子弹碰撞
        private void OnTriggerEnter2D(Collider2D collision) {
            //如果碰到敌人
            if(collision.gameObject.CompareTag("Enemy3"))
            {
                Debug.Log("碰到敌人3");
                Destroy(gameObject);                                     //销毁子弹
                FindObjectOfType<Enemy3_bullet>().TakeDamage();          //敌人3掉血
            }
            if(collision.gameObject.CompareTag("Enemy4"))
            {
                Debug.Log("碰到敌人4");
                Destroy(gameObject);                                     //销毁子弹
                FindObjectOfType<Enemy4_Bullet>().TakeDamage();          //敌人4掉血
            }
            if(collision.gameObject.CompareTag("Enemy5"))
            {
                Debug.Log("碰到敌人5");
                Destroy(gameObject);                                     //销毁子弹
                FindObjectOfType<Enemy5_Bullet>().TakeDamage();          //敌人5掉血
            }
            if(collision.gameObject.CompareTag("Enemy6"))
            {
                Debug.Log("碰到敌人6");
                Destroy(gameObject);                                     //销毁子弹
                FindObjectOfType<Enemy6_Bullet>().TakeDamage();          //敌人6掉血
            }
            if(collision.gameObject.CompareTag("Boss"))
            {
                Debug.Log("碰到Boss");
                Destroy(gameObject);
                FindObjectOfType<Boss>().TakeDamage();                   //Boss掉血
            }
            
        }

    }
}