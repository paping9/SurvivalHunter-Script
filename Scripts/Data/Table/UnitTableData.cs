using System;
using Defs;
using UnityEngine;

namespace Data.Table
{
    [Serializable]
    public class UnitData : TableRaw
    {
        public UnitType UnitType;
        public string Name;
        public int StatusGroupId = 0;
        public int SkillGroupId = 0;
        public int EquipSetGroupId = 0;
        public float AttackRange = 0;
        public string ResourcePath = "";
    }

    [CreateAssetMenu(fileName = "UnitTableData", menuName = "Table Data/Unit Table Data")]
    public class UnitTableData : BaseTableData<UnitData>
    {
    }

}