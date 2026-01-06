using System;
using Defs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Table
{
    [Serializable]
    public class DefaultEquipSetData : TableRaw
    {
        public int          GroupId;
        public EquipType    EquipType;
        public int          EquipId;
    }
    
    [CreateAssetMenu(fileName = "DefaultEquipSetTable", menuName = "Table Data/DefaultEquipSet Table Data")]
    public class DefaultEquipSetTableData : BaseTableData<DefaultEquipSetData>
    {
        
    }
}