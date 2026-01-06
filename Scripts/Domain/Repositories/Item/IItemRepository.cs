using System.Collections.Generic;

namespace Game.Repositories
{
    using Domain;
    
    public interface IItemRepository
    {
        Item GetItemById(int id);
        IEnumerable<Item> GetAllItems();
        void AddItem(Item item);
        void UpdateItem(Item item);
        void DeleteItem(int id);
    }
}