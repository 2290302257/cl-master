using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Alurax
{
    public class UGUIHelper
    {
        private static Sprite s_EmptySprite;
        public static Sprite EmptySprite
        {
            get
            {
                if (s_EmptySprite == null)
                {
                    s_EmptySprite = Resources.Load<Sprite>("EmptySprite");
                }
                return s_EmptySprite;
            }
        }
        
        private static Material s_Gray;
        public static Material Gray
        {
            get
            {
                if (s_Gray == null)
                {
                    s_Gray = new Material(Shader.Find("UI/Gray")) ;
                }
                return s_Gray;
            }
        }
        
    }
}