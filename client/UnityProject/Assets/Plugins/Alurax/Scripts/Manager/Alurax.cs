using System;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
    public class Alurax : MonoBehaviour
    {
        private static Alurax s_Inst;
        public static Alurax Inst
        {
            get
            {
                if (s_Inst == null)
                {
                 
                    s_Inst = GameObject.FindObjectOfType(typeof(Alurax)) as Alurax;
                    if (s_Inst == null)
                        s_Inst = new GameObject().AddComponent<Alurax>();
                    Quitting = false;
                }
                return s_Inst;
            }
        }
        public static bool Quitting { get; private set; }
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            
            if (UIRoot)
                DontDestroyOnLoad(UIRoot.gameObject);
        }

        public new void StopAllCoroutines()
        {
            Log.E("can not call this function!");
        }

        void OnApplicationQuit() 
        {
            Quitting = true;
        }
        
        //配置项
        [Header("UI_RUNTIME")]
        public Transform UIRoot;
        public Camera UICamera;
        public float UIDestroyTime=15f;

        //处理通用按钮点击事件
        public Action<Button> UIBtnClick;
        public Action<Toggle> UITogClick;
    }

}