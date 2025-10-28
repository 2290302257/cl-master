using System;
using System.Collections.Generic;

namespace Alurax
{
    public class EntityDataBoard
    {
        private EntityBase _entity;
        private Dictionary<Type, IEntityData> _datas;

        public EntityBase Entity => _entity;

        public EntityDataBoard(EntityBase entity)
        {
            _datas = new Dictionary<Type, IEntityData>();
            _entity = entity;
        }

        public T TryGet<T>() where T : IEntityData
        {
            Type type = typeof(T);
            if (_datas.TryGetValue(type, out IEntityData data))
            {
                return (T)data;
            }

            return default(T);
        }
        
        public T Get<T>() where T : IEntityData
        {
            Type type = typeof(T);
            if (!_datas.TryGetValue(type, out IEntityData data))
            {
                data = System.Activator.CreateInstance<T>();
                data.Entity = _entity;
                data.Init();
                data.Reset();
                _datas.Add(type, data);
            }

            return (T)data;
        }

        public void Reset()
        {
            foreach (var i in _datas.Values)
            {
                i.Reset();
            }
        }

        public void Destroy()
        {
            foreach (var i in _datas.Values)
            {
                i.Destroy();
                i.Entity = null;
            }
            
            _datas.Clear();
            _entity = null;
        }
    }
}
