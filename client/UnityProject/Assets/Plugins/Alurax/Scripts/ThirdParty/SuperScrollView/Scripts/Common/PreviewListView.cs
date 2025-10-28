using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alurax;

namespace SuperScrollView
{
    public class PreviewListView : MonoBehaviour,IExportProcess
    {
        public int count = 5;
        public int itemIndex = 0;
        void Start()
        {
            var list = GetComponent<LoopListView2>();
            if (list!=null && !list.ListViewInited)
            {
                list.InitListView(count,
                    (listView, index) =>
                    {
                        if (index < 0) return null;
                        return listView.NewListViewItem(listView.ItemPrefabDataList[itemIndex].mItemPrefab.name);
                    });
            }
            
            var grid = GetComponent<LoopGridView>();
            if (grid != null && !grid.ListViewInited)
            {
                grid.InitGridView(count, 
                    (gridView, index, row, column) =>
                {
                    if (index < 0) return null;
                    return gridView.NewListViewItem(gridView.ItemPrefabDataList[itemIndex].mItemPrefab.name);
                });
            }
            
        }
        
        public void Process()
        {
            if(this)
                GameObject.DestroyImmediate(this);
        }
    }
}
