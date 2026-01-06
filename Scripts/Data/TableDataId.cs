using Utils;

namespace Data
{
    public class TableDataId : Enumeration
    {
        public static TableDataId None = new TableDataId(0, nameof(None));

        public static TableDataId UnitData                  = new TableDataId(1, "TableData/UnitTable");
        public static TableDataId StageData                 = new TableDataId(2, "TableData/StageData");
        public static TableDataId EquipData                 = new TableDataId(3, "TableData/EquipTable");
        public static TableDataId WearableData              = new TableDataId(4, "TableData/WearableTable");
        public static TableDataId CharacterStatusData       = new TableDataId(5, "TableData/CharacterStatusTable");
        public static TableDataId EquipStatusData           = new TableDataId(6, "TableData/EquipStatusTable");
        public static TableDataId DefaultEquipSetTable      = new TableDataId(7, "TableData/DefaultEquipSetTable");
        
        public TableDataId(int id, string name) : base(id, name)
        {
        }
    }
}