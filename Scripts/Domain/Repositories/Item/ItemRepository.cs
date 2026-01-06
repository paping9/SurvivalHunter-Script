using System.Collections.Generic;

namespace Game.Repositories
{
    using Domain;
    public class ItemRepository : IItemRepository
    {
        private readonly Dictionary<int, Item> _items = new Dictionary<int, Item>();

        public Item GetItemById(int id)
        {
            _items.TryGetValue(id, out var item);
            return item;
        }

        public IEnumerable<Item> GetAllItems() => _items.Values;

        public void AddItem(Item item) => _items[item.Id] = item;

        public void UpdateItem(Item item) => _items[item.Id] = item;

        public void DeleteItem(int id) => _items.Remove(id);
    }
}