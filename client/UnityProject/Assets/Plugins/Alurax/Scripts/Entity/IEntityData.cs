using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public interface IEntityData
    {
        public EntityBase Entity { get; set; }
        
        /// <summary>
        /// 数据初始化，主要进行一些对象的创建工作(比如需要List或Dictionary)
        /// </summary>
        public void Init();

        /// <summary>
        /// 将数据都恢复成初始状态
        /// </summary>
        public void Reset();

        public void Destroy();
    }
}
