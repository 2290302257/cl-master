using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SuperScrollView
{
   
    [System.Serializable]
    public class TreeViewItemPrefabConfData
    {
        public GameObject mItemPrefab = null;
        public int mInitCreateCount = 0;
    }
    
    public class LoopTreeView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        Dictionary<string, TreeItemPool> mItemPoolDict = new Dictionary<string, TreeItemPool>();
        List<TreeItemPool> mItemPoolList = new List<TreeItemPool>();
        [SerializeField]
        List<TreeViewItemPrefabConfData> mItemPrefabDataList = new List<TreeViewItemPrefabConfData>();
        
        RectTransform mContainerTrans;

        int mItemTotalCount = 0;
        System.Func<LoopTreeView, int, LoopTreeViewItem> mOnGetItemByIndex;

        bool mIsDraging = false;
        public System.Action<PointerEventData> mOnBeginDragAction = null;
        public System.Action<PointerEventData> mOnDragingAction = null;
        public System.Action<PointerEventData> mOnEndDragAction = null;
        
        bool mListViewInited = false;
        int mListUpdateCheckFrameCount = 0;

        public bool ListViewInited => mListViewInited;
        private List<LoopTreeViewItem> mItemList = new List<LoopTreeViewItem>();
        
        public List<TreeViewItemPrefabConfData> ItemPrefabDataList
        {
            get
            {
                return mItemPrefabDataList;
            }
        }

        public int ItemTotalCount
        {
            get
            {
                return mItemTotalCount;
            }
        }

        public RectTransform ContainerTrans
        {
            get
            {
                return mContainerTrans;
            }
        }
        
        public bool IsDraging
        {
            get
            {
                return mIsDraging;
            }
        }

        public TreeViewItemPrefabConfData GetItemPrefabConfData(string prefabName)
        {
            foreach (TreeViewItemPrefabConfData data in mItemPrefabDataList)
            {
                if (data.mItemPrefab == null)
                {
                    Debug.LogError("A item prefab is null ");
                    continue;
                }
                if (prefabName == data.mItemPrefab.name)
                {
                    return data;
                }

            }
            return null;
        }

        /*
        LoopGridView method is to initiate the LoopGridView component. There are 4 parameters:
        itemTotalCount: the total item count in the GridView, this parameter must be set a value >=0 , then the ItemIndex can be from 0 to itemTotalCount -1.
        onGetItemByRowColumn: when a item is getting in the ScrollRect viewport, and this Action will be called with the item' index and the row and column index as the parameters, to let you create the item and update its content.
        settingParam: You can use this parameter to override the values in the Inspector Setting
        */
        public void InitTreeView(int itemTotalCount, System.Func<LoopTreeView, int, LoopTreeViewItem> onGetItemByRowColumn)
        {
            if (mListViewInited == true)
            {
                Debug.LogError("LoopGridView.InitListView method can be called only once.");
                return;
            }
            mListViewInited = true;
            if (itemTotalCount < 0)
            {
                Debug.LogError("itemTotalCount is  < 0");
                itemTotalCount = 0;
            }
            
            mContainerTrans = gameObject.GetComponent<RectTransform>();
            
            InitItemPool();
            mOnGetItemByIndex = onGetItemByRowColumn;
            mItemTotalCount = itemTotalCount;
        }


        /*
        This method may use to set the item total count of the GridView at runtime. 
        this parameter must be set a value >=0 , and the ItemIndex can be from 0 to itemCount -1.  
        If resetPos is set false, then the ScrollRect’s content position will not changed after this method finished.
        */
        public void SetTreeItemCount(int itemCount, bool resetPos = true)
        {
            if(itemCount < 0)
            {
                return;
            }
            if(itemCount == mItemTotalCount)
            {
                return;
            }

            mItemTotalCount = itemCount;
            if (mItemTotalCount == 0)
            {
                RecycleAllItem();
                ClearAllTmpRecycledItem();
                return;
            }
            UpdateTreeViewContent();
            ClearAllTmpRecycledItem();
            if (resetPos)
            {
                // nothing
            }
        }

       //fetch or create a new item form the item pool.
        public LoopTreeViewItem NewListViewItem(string itemPrefabName)
        {
            TreeItemPool pool = null;
            if (mItemPoolDict.TryGetValue(itemPrefabName, out pool) == false)
            {
                return null;
            }
            LoopTreeViewItem item = pool.GetItem();
            RectTransform rf = item.GetComponent<RectTransform>();
            rf.SetParent(mContainerTrans);
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            item.ParentTreeView = this;
            return item;
        }

        public void ClearAllShownItems()
        {
            RecycleAllItem();
            ClearAllTmpRecycledItem();
        }


        /*
        To update a item by itemIndex.if the itemIndex-th item is not visible, then this method will do nothing.
        Otherwise this method will call RefreshItemByRowColumn to do real work.
        */
        public void RefreshItemByItemIndex(int itemIndex)
        {
            if(itemIndex < 0 || itemIndex >= ItemTotalCount)
            {
                return;
            }
            
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            
            LoopTreeViewItem curItem = mItemList[itemIndex];
            if(curItem == null)
            {
                return;
            }
            LoopTreeViewItem newItem = GetNewItemByIndex(itemIndex);
            if (newItem == null)
            {
                return;
            }
            Vector3 pos = curItem.CachedRectTransform.anchoredPosition3D;
            mItemList[itemIndex] = newItem;
            RecycleItemTmp(curItem);
            newItem.CachedRectTransform.anchoredPosition3D = pos;
            ClearAllTmpRecycledItem();
        }
        
        //update all visible items.
        public void RefreshAllShownItem()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            RecycleAllItem();
            UpdateTreeViewContent();
        }


        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            mIsDraging = true;
            if (mOnBeginDragAction != null)
            {
                mOnBeginDragAction(eventData);
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            mIsDraging = false;
            if (mOnEndDragAction != null)
            {
                mOnEndDragAction(eventData);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            if (mOnDragingAction != null)
            {
                mOnDragingAction(eventData);
            }
        }
        
        //get the shown item of itemIndex, if this item is not shown,then return null.
        public LoopTreeViewItem GetShownItemByItemIndex(int itemIndex)
        {
            if(itemIndex < 0 || itemIndex >= ItemTotalCount)
            {
                return null;
            }
            if(mItemList.Count == 0)
            {
                return null;
            }

            return mItemList[itemIndex];
        }
        
        public void ClearAllTmpRecycledItem()
        {
            int count = mItemPoolList.Count;
            for (int i = 0; i < count; ++i)
            {
                mItemPoolList[i].ClearTmpRecycledItem();
            }
        }
        
        public void RecycleAllItem()
        {
            for (int i = mItemList.Count-1; i >= 0; i--)
            {
                RecycleItemTmp(mItemList[i]);
            }
            // foreach (LoopTreeViewItem item in mItemList)
            // {
            //     RecycleItemTmp(item);
            // }
            mItemList.Clear();
        }

        public void UpdateTreeViewContent()
        {
            mListUpdateCheckFrameCount++;
            if (mItemTotalCount == 0)
            {
                if (mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return;
            }
            
            for (int i = mItemList.Count; i < mItemTotalCount; i++)
            {
                LoopTreeViewItem item = GetNewItemByIndex(i);
                if(item == null)
                {
                    Debug.LogError("GentNewItem is null.");
                    return;
                }

                mItemList.Add(item);
            }

            for (int i = mItemList.Count - 1; i >= mItemTotalCount; i--)
            {
                RecycleItemTmp(mItemList[i]);
                mItemList.RemoveAt(i);
            }
        }

        void RecycleItemTmp(LoopTreeViewItem item)
        {
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemPrefabName))
            {
                return;
            }
            TreeItemPool pool = null;
            if (mItemPoolDict.TryGetValue(item.ItemPrefabName, out pool) == false)
            {
                return;
            }
            pool.RecycleItem(item);

        }
        
        void InitItemPool()
        {
            foreach (TreeViewItemPrefabConfData data in mItemPrefabDataList)
            {
                if (data.mItemPrefab == null)
                {
                    Debug.LogError("A item prefab is null ");
                    continue;
                }
                string prefabName = data.mItemPrefab.name;
                if (mItemPoolDict.ContainsKey(prefabName))
                {
                    Debug.LogError("A item prefab with name " + prefabName + " has existed!");
                    continue;
                }
                RectTransform rtf = data.mItemPrefab.GetComponent<RectTransform>();
                if (rtf == null)
                {
                    Debug.LogError("RectTransform component is not found in the prefab " + prefabName);
                    continue;
                }
                LoopTreeViewItem tItem = data.mItemPrefab.GetComponent<LoopTreeViewItem>();
                if (tItem == null)
                {
                    data.mItemPrefab.AddComponent<LoopTreeViewItem>();
                }
                TreeItemPool pool = new TreeItemPool();
                pool.Init(data.mItemPrefab, data.mInitCreateCount, mContainerTrans);
                mItemPoolDict.Add(prefabName, pool);
                mItemPoolList.Add(pool);
            }
        }
        
        LoopTreeViewItem GetNewItemByIndex(int itemIndex)
        {
            if(itemIndex < 0 || itemIndex >= ItemTotalCount)
            {
                return null;
            }
            LoopTreeViewItem newItem = mOnGetItemByIndex(this, itemIndex);
            if (newItem == null)
            {
                return null;
            }
            newItem.ItemIndex = itemIndex;
            newItem.ItemCreatedCheckFrameCount = mListUpdateCheckFrameCount;
            return newItem;
        }

        void Update()
        {
            if(mListViewInited == false)
            {
                return;
            }

            UpdateTreeViewContent();
            ClearAllTmpRecycledItem();
        }
        

    }

}
