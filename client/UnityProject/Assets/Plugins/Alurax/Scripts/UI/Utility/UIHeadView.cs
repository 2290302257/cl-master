using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alurax
{
    [RequireComponent(typeof(RectTransform))]
    public class UIHeadView : MonoBehaviour ,IExport
    {
        private RectTransform m_Rect;
        private List<HeadData> m_Head = new List<HeadData>();
        public GameObject Prefab;
        private void Awake()
        {
            m_Rect = GetComponent<RectTransform>();
        }
        class HeadData
        {
            public Transform world;
            public RectTransform ui;
            public UIHandler handler;
            public Vector3 offset;
        }

        public T Add<T>(Transform world) where T : UIHandler
        {
            return Add<T>(world, Vector3.zero);
        }
        
        public T Add<T>(Transform world,Vector3 offset) where T : UIHandler
        {
            var clone = GameObject.Instantiate(Prefab);
            clone.gameObject.SetActive(true);
            clone.transform.SetParent(transform,false);
            var handler = UIBindItem.Make<T>(clone.transform);
            HeadData headData = new HeadData()
            {
                ui = clone.transform as RectTransform,
                handler = handler,
                world = world,
                offset = offset
            };
            m_Head.Add(headData);
            return handler;
        }

        public T Get<T>(int index) where T : UIHandler
        {
            if (index < m_Head.Count)
                return (T)m_Head[index].handler;
            return null;
        }
        
        public T Get<T>(Transform world) where T : UIHandler
        {
            foreach (var head in m_Head)
            {
                if (head != null && head.handler != null)
                    return (T)head.handler;
            }
            return null;
        }

        public void Remove(int index)
        {
            if (index < m_Head.Count)
            {
                var head = m_Head[index];
                if (head.ui)
                    Destroy(head.ui.gameObject);
                m_Head.RemoveAt(index);
            }
        }
        public void Remove(Transform world)
        {
            for (int i = 0; i < m_Head.Count; i++)
            {
                var head = m_Head[i];
                if (head != null && head.world == world)
                {
                    if (head.ui)
                        Destroy(head.ui.gameObject);
                    m_Head.RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveAll()
        {
            foreach (var head in m_Head)
            {
                if (head!=null && head.ui!=null)
                    Destroy(head.ui.gameObject);
            }
            m_Head.Clear();
        }


        private void OnDestroy()
        {
            RemoveAll();
        }

        void Update()
        {
            Camera mainCamera = Camera.main;
            Camera uiCamera = Alurax.Inst.UICamera;
            foreach (var head in m_Head)
            {
                if (head!= null&& head.ui&&head.world)
                {
                    Vector3 worldPos = head.world.position + head.offset;
                    Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Rect, screenPos, uiCamera,
                            out Vector2 localPos))
                    {
                        head.ui.anchoredPosition = localPos;
                    }
                }
            }
        }
    }
}
