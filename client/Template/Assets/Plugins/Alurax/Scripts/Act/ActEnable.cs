using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    [ActName("显示隐藏")]
    public class ActEnable : ActBase
    { 
        [LabelText("相对路径")]
        public string Path;
        [LabelText("显示隐藏")]
        public bool Enable;
        
        private GameObject Obj;

        protected override void OnCreated()
        {
            Obj = GetObjFromPath(Path);
        }
        
        protected override void OnStart(float offsetTime)
        {
            // Debug.Log($"{Obj.name} start {Obj.activeSelf} -> {Enable}");
            Obj?.SetActive(Enable);
        }

        protected override void OnFinish()
        {
            // Debug.Log($"{Obj.name} finish {Obj.activeSelf} -> {!Enable}");
            Obj?.SetActive(!Enable);
        }

        protected override void OnUpdate(float curTime)
        {
        }

        protected override void OnDestroyed()
        {
            Obj = null;
        }
    }
}
