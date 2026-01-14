using System;
using UnityEngine;

namespace Data.Table
{
    
    
    [Serializable]
    public class EquipStatusData : StatusData
    {
        public int GroupId;
    }
    
    [CreateAssetMenu(fileName = "EquipStatusTable", menuName = "Table Data/EquipStatus Table Data")]
    public class EquipStatusTable : BaseTableData<EquipStatusData>
    {
        
    }
}