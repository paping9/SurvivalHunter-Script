using System;
using Defs;
using UnityEngine;

namespace Data.Table
{
    [Serializable]
    public class ItemData : TableRaw
    {
        public ItemType ItemType;
        public string ItemName;
        public string AtlasName;
        public string IconName;
    }
    
    [CreateAssetMenu(fileName = "ItemTableData", menuName = "Table Data/Item Table Data")]
    public class ItemTableData : BaseTableData<ItemData>
    {
        
    }
}