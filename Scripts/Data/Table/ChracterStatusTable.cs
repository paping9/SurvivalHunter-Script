using System;
using UnityEngine;

namespace Data.Table
{
    [Serializable]
    public class ChracterStatusData : StatusData
    {
        public int GroupId;
    }
    
    [CreateAssetMenu(fileName = "ChracterStatusTable", menuName = "Table Data/ChracterStatus Table Data")]
    public class ChracterStatusTable : BaseTableData<ChracterStatusData>
    {
        
    }
}