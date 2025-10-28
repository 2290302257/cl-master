using UnityEngine;

namespace Alurax
{
    public class UIViewSub : UIView
    {
        private InternalBind bind;
        public void Open()
        {
            bind.Open();
        }
        
        public void Close()
        {
            bind.Close();
        }
        
        /*******************************************************************\
        | Internal Methods (Don't call these methods)                       |
        \*******************************************************************/
        static internal void InternalInit(UIViewSub view,GameObject go)
        {
            view.gameObject = go;
            view.transform = go.transform;
            view.bind = go.GetComponent<InternalBind>();
            if (!view.bind)
            {
                view.bind=go.AddComponent<InternalBind>();
                view.bind.view = view;
            }
        }
        static internal new void InternalOpened(UIView view)
        {
            if (view.m_SubPage != null)
            {
                foreach (var sub in view.m_SubPage)
                {
                    if (sub.gameObject.activeSelf)
                    {
                        sub.Open();
                    }
                }
            }
        }
        static internal new void InternalAnimOpened(UIView view)
        {
            if (view.m_SubPage != null)
            {
                foreach (var sub in view.m_SubPage)
                {
                    if (sub.gameObject.activeSelf)
                    {
                        sub.OnAnimOpened();
                    }
                }
            }
        }
        
        
        static internal new void InternalFocusChanged(UIView view,bool active)
        {
            if (view.m_SubPage != null)
            {
                foreach (var sub in view.m_SubPage)
                {
                    sub.OnFocusChanged(active);
                }
            }
        }

        static internal new void InternalUpdate(UIView view)
        {
            if (view.m_SubPage != null)
            {
                foreach (var sub in view.m_SubPage)
                {
                    if (sub.EnableUpdate)
                    {
                        var time = Time.unscaledTime;
                        if (time - sub.LasetUpdateTime >= sub.UpdateInterval)
                        {
                            sub.OnUpdate();
                            sub.LasetUpdateTime = time;
                        }
                    }
                }
            }
        }

        static internal new void InternalClosed(UIView view)
        {
            if (view.m_SubPage != null)
            {
                foreach (var sub in view.m_SubPage)
                {
                    sub.Close();
                }
            }
        }
        static internal new void InternalDestroyed(UIView view)
        {
            if (view.m_SubPage != null)
            {
                foreach (var sub in view.m_SubPage)
                {
                    sub.OnDestroyed();
                }
            }
        }
        
        class InternalBind : MonoBehaviour
        {
            internal UIViewSub view;
            private bool _opened;
            private bool _created;

            public void Open()
            {
                if(gameObject.activeSelf)
                    InternalOpen();
                else
                    gameObject.SetActive(true);
            }
            public void Close()
            {
                if(!gameObject.activeSelf)
                    InternalClose();
                else
                    gameObject.SetActive(false);
            }
            
            private void OnEnable()
            {
                InternalOpen();
            }
            
            private void OnDisable()
            {
                InternalClose();
            }

            private void InternalOpen()
            {
                if (view == null) return;
                if (!_created)
                {
                    _created = true;
                    view.OnCreated();
                }
                
                if (!_opened)
                {
                    _opened = true;
                    view.OnOpened();
                }
            }

            private void InternalClose()
            {
                if (view == null) return;
                if (_opened)
                {
                    _opened = false;
                    view.OnClosed();
                }
            }
        }
    }
}