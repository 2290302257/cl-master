using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Alurax
{
    public class UIContainer : UIBehaviour
    {
        public List<GameObject> itemPrefabList = new List<GameObject>();
        public bool IsInitialized => _updateItem!= null;
        public int Count => _itemList.Count;
        
        class Item
        {
            public GameObject ItemGo;
            public string ItemName;
            public object ItemData;
        }
        private GameObject _poolRoot;
        private string _defaultName;
        private Dictionary<string, GameObject> _dictionary ;
        private Dictionary<string, Queue<GameObject>> _dictPool=new Dictionary<string, Queue<GameObject>>();
        private Action<GameObject,object> _updateItem;
        private Action<GameObject,object> _returnItem;
        private List<Item> _itemList = new List<Item>();
        
        public string DefaultName{
            get
            {
                if (string.IsNullOrEmpty(_defaultName))
                {
                    if(itemPrefabList.Count>0)
                        _defaultName = itemPrefabList[0].name;
                }
                return _defaultName;
            }
            set
            {
                _defaultName = value;
            }
        }

        private Dictionary<string, GameObject> Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<string, GameObject>();
                    foreach (var prefab in itemPrefabList)
                    {
                        _dictionary[prefab.name] = prefab;
                        prefab.SetActive(false);
                    }
                }

                return _dictionary;
            }
        }

        private GameObject PoolRoot
        {
            get
            {
                if (_poolRoot == null)
                {
                    _poolRoot = new GameObject("pool_root");
                    _poolRoot.SetParent(gameObject,false);
                }

                return _poolRoot;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            var tmpDic = Dictionary;
            var tmpDefName = DefaultName;
            var tmpPoolRoot = PoolRoot;
        }
    
        public void InitContainer(Action<GameObject,object> updateItem)
        {
            Clear();
            _updateItem = updateItem;
        }
        
        public void SetReturnPoolCallback(Action<GameObject,object> returnPool)
        {
            _returnItem = returnPool;
        }
        
        public GameObject AddData(object data)
        {
            return AddData(DefaultName, data);
        }
        
        public GameObject AddData(string itemName, object data)
        {
            var go = GetPool(itemName);
            _itemList.Add(new Item(){ ItemGo = go , ItemName = itemName , ItemData = data});
            _updateItem?.Invoke(go,data);

            return go;
        }

        public void SetData(int index, object data)
        {
            if (index < _itemList.Count)
            {
                _itemList[index].ItemData = data;
                RefreshAt(index);
            }
        }
        
        public GameObject Insert(int index, object data)
        {
            return Insert(DefaultName, index, data);
        }
        
        public GameObject Insert(string itemName, int index, object data)
        {
            var go = GetPool(itemName);
            var item = new Item() { ItemGo = go, ItemName = itemName };
            if (index < _itemList.Count)
            {
                var cur = _itemList[index];
                go.transform.SetSiblingIndex(cur.ItemGo.transform.GetSiblingIndex());
                _itemList.Insert(index,item);
            }
            else
            {
                _itemList.Add(item);
            }
            _updateItem?.Invoke(go,data);

            return go;
        }

        public void RemoveAt(int index)
        {
            if (index < _itemList.Count)
            {
                ReturnPool(_itemList[index]);
                _itemList.RemoveAt(index);
            }
        }

        public void Remove(object data)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].ItemData == data)
                {
                    RemoveAt(i);
                    return;
                }
            }
        }
        
        public bool Contains(object data)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].ItemData == data)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void RemoveIf(Func<GameObject, bool> checkCondition)
        {
            for (int i = _itemList.Count - 1; i >= 0; i--)
            {
                if (checkCondition(_itemList[i].ItemGo))
                {
                    RemoveAt(i);
                }
            }
        }

        public GameObject GetAt(int index)
        {
            if (index < _itemList.Count)
            {
                return _itemList[index].ItemGo;
            }
            return null;
        }
        
        public object GetDataAt(int index)
        {
            if (index < _itemList.Count)
            {
                return _itemList[index].ItemData;
            }
            return null;
        }

        public void RefreshAt(int index)
        {
            if (index < _itemList.Count)
            {
                _updateItem?.Invoke(_itemList[index].ItemGo,_itemList[index].ItemData);
            }
        }
        
        public void Refresh(object data)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].ItemData == data)
                {
                    RefreshAt(i);
                    return;
                }
            }
        }
        
        public void RefreshAll()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                RefreshAt(i);
            }
        }
        
        public void Clear()
        {
            if (_itemList == null || _itemList.Count < 1)
            {
                return;
            }
            
            int loopTime = _itemList.Count;
            for (int i = 0; i < loopTime; ++i)
            {
                ReturnPool(_itemList[i]);
            }
            _itemList.Clear();
        }
        
        GameObject GetPool(string itemName)
        {
            GameObject go = null;
            if (_dictPool.TryGetValue(itemName, out var list) && list.Count >0)
                go = list.Dequeue();
            if(go==null)
                go= GameObject.Instantiate(Dictionary[itemName], transform, false);
            go.SetParent(gameObject, false);
            go.SetActive(true);
            return go;
        }

        void ReturnPool(Item item)
        {
            var go = item.ItemGo;
            if (go)
            {
                var itemName=item.ItemName;
                _returnItem?.Invoke(go,item.ItemData);
                go.SetParent(PoolRoot, false);
                go.SetActive(false);
                if (!_dictPool.TryGetValue(itemName, out var list))
                {
                    _dictPool[itemName] = new Queue<GameObject>();
                    list = _dictPool[itemName];
                }
                list.Enqueue(go);
            }
        }
    }
}
