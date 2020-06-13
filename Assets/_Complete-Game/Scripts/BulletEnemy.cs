using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class BulletEnemy : MonoBehaviour
    {
        public float bulletSpeed = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // GetComponent<Rigidbody2D>().velocity = Vector2.down * bulletSpeed;
        }

        //子弹碰撞
        private void OnTriggerEnter2D(Collider2D collision) {
            //如果碰到玩家
            if(collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("碰到玩家");
                Destroy(gameObject);                                   //销毁子弹
                FindObjectOfType<Player>().LoseFood(10);               //子弹击中玩家，玩家点血10点 
            }
        }
    }
}