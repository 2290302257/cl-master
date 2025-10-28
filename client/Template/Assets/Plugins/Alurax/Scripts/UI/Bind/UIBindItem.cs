using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class UIBindItem : MonoBehaviour
    {
        public List<Component> Bind =new List<Component>();
        private UIHandler m_Handler;
        
        public static T Make<T>(Component item) where T : UIHandler
        {
            var bindItem = item.GetComponent<UIBindItem>() ?? item.gameObject.AddComponent<UIBindItem>();
            return bindItem.GetItemData<T>();
        }
        
        private T GetItemData<T>() where T : UIHandler
        {
            if (m_Handler == null)
            {
                m_Handler = (T)Activator.CreateInstance(typeof(T));
                UIHandler.InternalCreated(m_Handler,gameObject);
                if(gameObject.activeSelf)
                    UIHandler.InternalOpened(m_Handler);
            }
            return (T)m_Handler;
        }

        private void OnEnable()
        {
            if (m_Handler != null)
                UIHandler.InternalOpened(m_Handler);
        }

        private void OnDisable()
        {
            if (m_Handler != null)
                UIHandler.InternalClosed(m_Handler);
        }

        private void OnDestroy()
        {
            if (m_Handler != null)
            {
                UIHandler.InternalDestroyed(m_Handler);
                m_Handler = null;
            }
        }
    }
}