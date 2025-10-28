using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
    public abstract class UIHandler 
    {
	    public GameObject gameObject
	    {
		    get; protected set;
	    }

	    public Transform transform
	    {
		    get; protected set;
	    }
	    
	    private TaskManager.Timer __OnCreatedTimer,__OnOpenedTimer;
	    protected TaskManager.Timer OnCreatedTimer
	    {
		    get
		    {
			    if (__OnCreatedTimer == null) __OnCreatedTimer = new TaskManager.Timer();
			    return __OnCreatedTimer;
		    }
	    }

	    protected TaskManager.Timer m_OnTimer
	    {
		    get
		    {
			    if (__OnOpenedTimer == null) __OnOpenedTimer = new TaskManager.Timer();
			    return __OnOpenedTimer;
		    }
	    }
	    
	    /******************************************************\
		| View Life Circle Methods                             |
		\******************************************************/
	    virtual protected void OnCreated()
	    {
	    }
	    virtual protected void OnOpened()
	    {
	    }
	    virtual protected void OnClosed()
	    {
		    __OnOpenedTimer?.Clear();
	    }
	    virtual protected void OnDestroyed()
	    {
		    __OnCreatedTimer?.Clear();
	    }
	    
        /*********************************************************\
        | UI Event Handlers                                       |
        \*********************************************************/
        virtual protected void OnButtonClick(Component comp)
        {
        }
		
        // Dropdown
        virtual protected void OnDropdownChanged(Component comp, int value)
        {
        }
		
        //ScrollBar
        virtual protected void OnScrollBarChanged(Component comp, float value)
        {
        }
		
        //Slider 
        virtual protected void OnSliderChanged(Component comp, float value)
        {
        }
		
        // Toggle
        virtual protected void OnToggleChanged(Component comp, bool value)
        {
        }
		
        // InputField
        virtual protected void OnInputFieldChanged(Component comp, string value)
        {
			
        }
        /*******************************************************************\
		| Internal Methods (Don't call these methods)                       |
		\*******************************************************************/
        static internal void InternalCreated(UIHandler handler, GameObject gameObject)
        {
	        handler.gameObject = gameObject;
	        handler.transform = gameObject.transform;
	        handler.OnCreated();
        }
		
        static internal void InternalOpened(UIHandler handler)
        {
	        handler.OnOpened();
        }
        
        static internal void InternalClosed(UIHandler handler)
        {
	        handler.OnClosed();
        }
		
        static internal void InternalDestroyed(UIHandler handler)
        {
	        handler.OnDestroyed();
        }
        
        protected void OnBindEvents(List<Component> comps)
        {
	        for (int i = 0, count = comps.Count; i < count; i++) {
		        Component comp = comps[i];
		        if (comp is Button button) {
			        button.onClick.AddListener(delegate {
				        Alurax.Inst.UIBtnClick?.Invoke(button);
				        OnButtonClick(comp);
			        });
		        } else if (comp is Slider slider) {
			        slider.onValueChanged.AddListener(delegate (float value) {
				        OnSliderChanged(comp, value);
			        });
		        } else if (comp is Scrollbar scrollbar) {
			        scrollbar.onValueChanged.AddListener(delegate (float value) {
				        OnScrollBarChanged(comp, value);
			        });
		        } else if (comp is Toggle toggle) {
			        toggle.onValueChanged.AddListener(delegate (bool value) {
				        if(value) Alurax.Inst.UITogClick?.Invoke(toggle);
				        OnToggleChanged(comp, value);
			        });
		        } else if (comp is Dropdown dropdown) {
			        dropdown.onValueChanged.AddListener(delegate (int value) {
				        OnDropdownChanged(comp, value);
			        });
		        } else if (comp is InputField inputField) {
			        inputField.onEndEdit.AddListener(delegate (string value) {
				        OnInputFieldChanged(comp, value);
			        });
		        } else if (comp is TMP_InputField tmpInputField) {
			        tmpInputField.onEndEdit.AddListener(delegate (string value) {
				        OnInputFieldChanged(comp, value);
			        });
		        } 
	        }
        }
    }
}
