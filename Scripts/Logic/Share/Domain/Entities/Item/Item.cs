using Data.Table;
using Defs;

namespace Game.Domain
{
    public class Item : IEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public ItemType ItemType { get; private set; } // 예: "무기", "방어구", "소비품" 등
        
        public Item(int id, ItemData itemData)
        {
            Id = id;
            Name = itemData.ItemName;
            ItemType = itemData.ItemType;
        }
    }
}