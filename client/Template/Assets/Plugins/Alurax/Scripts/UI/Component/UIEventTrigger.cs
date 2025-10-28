using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Serialization;

namespace Alurax
{
    public class UIEventTrigger : EventTrigger 
    {
        public Action<PointerEventData> onBeginDrag;
        public Action<PointerEventData> onDrag;
        public Action<PointerEventData> onDrog;
        public Action<PointerEventData> onEndDrag;
        public Action<PointerEventData> onInitializePotentialDrag;
        public Action<AxisEventData> onMove;
        public Action<PointerEventData> onClick;
        public Action<PointerEventData> onPointerDown;
        public Action<PointerEventData> onPointerEnter;
        public Action<PointerEventData> onPointerExit;
        public Action<PointerEventData> onPointerUp;
        public Action<PointerEventData> onScroll;
        public Action<BaseEventData> onSelect;
        public Action<BaseEventData> onDeselect;
        public Action<BaseEventData> onSubmit;
        public Action<BaseEventData> onCancel;
        public Action<BaseEventData> onUpdateSelected;
        
	    public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);
            if (onBeginDrag != null) {
                onBeginDrag(eventData);
            }
        }
        
	    public override void OnDrag(PointerEventData eventData) {
            base.OnDrag(eventData);
            if (onDrag != null) {
                onDrag(eventData);
            }
        }
        
	    public override void OnDrop(PointerEventData eventData) {
            base.OnDrop(eventData);
            if (onDrog != null) {
                onDrog(eventData);
            }
        }

	    public override void OnEndDrag(PointerEventData eventData) {
            base.OnEndDrag(eventData);
            if (onEndDrag != null) {
                onEndDrag(eventData);
            }
        }
        
	    public override void OnInitializePotentialDrag(PointerEventData eventData) {
            base.OnInitializePotentialDrag(eventData);
            if (onInitializePotentialDrag != null) {
                onInitializePotentialDrag(eventData);
            }
        }
        
	    public override void OnMove(AxisEventData eventData) {
            base.OnMove(eventData);
            if (onMove != null) {
                onMove(eventData);
            }
        }
        
	    public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            if (onClick != null) {
                onClick(eventData);
            }
        }
        
	    public override void OnPointerDown(PointerEventData eventData) {
            base.OnPointerDown(eventData);
            if (onPointerDown != null) {
                onPointerDown(eventData);
            }
        }
        
	    public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            if (onPointerEnter != null) {
                onPointerEnter(eventData);
            }
        }
        
	    public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            if (onPointerExit != null) {
                onPointerExit(eventData);
            }
        }

	    public override void OnPointerUp(PointerEventData eventData) {
            base.OnPointerUp(eventData);
            if (onPointerUp != null) {
                onPointerUp(eventData);
            }
        }
	    public override void OnScroll(PointerEventData eventData) {
            base.OnScroll(eventData);
            if (onScroll != null) {
                onScroll(eventData);
            }
        }

	    public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
            if (onSelect != null) {
                onSelect(eventData);
            }
        }
        
        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);
            if (onDeselect != null) {
                onDeselect(eventData);
            }
        }
        
	    public override void OnSubmit(BaseEventData eventData) {
            base.OnSubmit(eventData);
            if (onSubmit != null) {
                onSubmit(eventData);
            }
        }
        
        public override void OnCancel(BaseEventData eventData) {
            base.OnCancel(eventData);
            if (onCancel != null) {
                onCancel(eventData);
            }
        }

	    public override void OnUpdateSelected(BaseEventData eventData) {
            base.OnUpdateSelected(eventData);
            if (onUpdateSelected != null) {
                onUpdateSelected(eventData);
            }
        }
    }
}
