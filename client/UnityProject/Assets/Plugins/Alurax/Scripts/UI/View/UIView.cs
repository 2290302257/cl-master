using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
	public abstract class UIView : UIHandler 
	{
        protected virtual bool EnableUpdate
        {
            get; set;
        }

        protected float LasetUpdateTime;
        protected virtual float UpdateInterval
		{
			get; set;
		} = 0.1f;
        
		private ActionManager __OnCreatedAction,__OnOpenedAction;
		protected ActionManager OnCreatedAction
		{
			get
			{
				if (__OnCreatedAction == null) __OnCreatedAction = new ActionManager();
				return __OnCreatedAction;
			}
		}

		protected ActionManager m_OnAction
		{
			get
			{
				if (__OnOpenedAction == null) __OnOpenedAction = new ActionManager();
				return __OnOpenedAction;
			}
		}
		
        internal List<UIViewSub> m_SubPage;
        
		virtual protected void OnBackButtonPressed()
		{
		}

		virtual protected void OnLocalized()
		{
		}

		/******************************************************\
		| View Life Circle Methods                             |
		\******************************************************/
		
		virtual protected void OnAnimOpened()
		{
		}
		
		virtual protected void OnFocusChanged(bool focus)
		{
		}
		
        virtual protected void OnUpdate()
		{
		}
        
        override protected void OnClosed()
		{
			base.OnClosed();
			__OnOpenedAction?.Clear();
		}
        
		override protected void OnDestroyed()
		{
			base.OnDestroyed();
			__OnCreatedAction?.Clear();
		}

		/******************************************************\
		| View Others Methods                             |
		\******************************************************/
		virtual protected void OnOverrideCanvas(Canvas canvas)
		{
		}
		
		/*******************************************************************\
		| Internal Methods (Don't call these methods)                       |
		\*******************************************************************/
		static internal void InternalCreated(UIView view, GameObject gameObject)
		{
			UIHandler.InternalCreated(view,gameObject);
		}
		
		static internal void InternalOpened(UIView view)
		{
			UIViewSub.InternalOpened(view);
			UIHandler.InternalOpened(view);
		}
		
		static internal void InternalAnimOpened(UIView view)
		{
			UIViewSub.InternalAnimOpened(view);
			view.OnAnimOpened();
		}
		
		static internal void InternalFocusChanged(UIView view, bool active)
		{
			UIViewSub.InternalFocusChanged(view,active);
			view.OnFocusChanged(active);
		}
		
        static internal void InternalUpdate(UIView view)
        {
	        UIViewSub.InternalUpdate(view);
			if (view.EnableUpdate)
			{
				var time = Time.unscaledTime;
                if (time - view.LasetUpdateTime >= view.UpdateInterval)
				{
					view.OnUpdate();
					view.LasetUpdateTime = time;
                }
			}
        }

		static internal void InternalClosed(UIView view)
		{
			UIViewSub.InternalClosed(view);
			UIHandler.InternalClosed(view);
		}
		
		static internal void InternalDestroyed(UIView view)
		{
			UIViewSub.InternalDestroyed(view);
			UIHandler.InternalDestroyed(view);
		}
		
		static internal void InternalOverrideCanvas(UIView view,Canvas canvas)
		{
			view.OnOverrideCanvas(canvas);
		}
		
		/*******************************************************************\
		| Internal protected Methods (Don't call these methods)             |
		\*******************************************************************/
		protected void OnBindSubPage(UIViewSub subView,GameObject sGameObject)
		{
			if(m_SubPage==null)
				m_SubPage = new List<UIViewSub>();
			UIViewSub.InternalInit(subView,sGameObject);
			m_SubPage.Add(subView);
		}
	}
}
