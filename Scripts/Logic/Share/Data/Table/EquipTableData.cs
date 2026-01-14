using System;
using Defs;
using UnityEngine;

namespace Data.Table
{
    [Serializable]
    public class EquipData : ItemData
    {
        public EquipType EquipType;
        public string PivotName;
        public string AddressablePath;
        public int StatusId;

        public EquipData()
        {
            ItemType = ItemType.Equip;
        }
    }
    
    [CreateAssetMenu(fileName = "EquipTableData", menuName = "Table Data/Equip Table Data")]
    public class EquipTableData : BaseTableData<EquipData>
    {
        
    }
}