using Data.Table;
using Defs;

namespace Game.Domain
{
    public class EquipItem : Item
    {
        public int StatusId { get; private set; }
        public EquipType EquipType { get; private set; }
        public string PivotName { get; private set; }
        
        public EquipItem(int id, EquipData equipData) : base(id, equipData as ItemData)
        {
            EquipType = equipData.EquipType;
            PivotName = equipData.PivotName;
        }
    }
}