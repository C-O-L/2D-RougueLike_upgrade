using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    //按下F2显示小组成员列表的类
    public class DisplayTeamList : MonoBehaviour
    {
        public static GameObject text;				    //定义公共静态变量用于另外脚本的引用
        // Start is called before the first frame update
        public void Start()
        {
            text = GameObject.Find("MemberText");       //MemberText存储小组成员列表。
            text.SetActive (false);				        //保持小组成员列表的隐藏状态
        }

        // Update is called once per frame
        public void Update()
        {
            
        }

        // OnGUI用于实现（按下F2显示小组成员列表，松开F2隐藏小组成员列表）（作业1）
        void OnGUI()  
        {  
            if (Input.GetKeyDown(KeyCode.F2)){          //按下F2      
                Event e = Event.current;  
                if (e.isKey)  
                {  
                    text.SetActive (true);              //显示小组成员列表
                }  
            }
            if(Input.GetKeyUp(KeyCode.F2)){             //松开F2
                Event e = Event.current;  
                if (e.isKey)  
                {  
                    text.SetActive (false);              //隐藏小组成员列表
                }  
            }  
        }  
    }
}