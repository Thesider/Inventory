using Inventory.Data;
using Inventory.Enum;
using Inventory.Models;
using Inventory.ViewModels.Interface;

namespace Inventory.ViewModels
{
    public class ItemDetailViewModel : IItemDetailViewModel
    {
        private readonly DatabaseService _db;
        public Items Item { get; private set; } = new();

        public ItemDetailViewModel(DatabaseService db)
        {
            _db = db;
        }

        public async Task LoadItem(int id)
        {
            Item = await _db.GetItemAsync(id) ?? new Items();
        }

        public void NewItem()
        {
            Item = new Items();
        }

        public async Task SaveItem()
        {
            // Update status based on quantity
            if (Item.Quantity <= 1)
            {
                Item.ItemStatus = Status.OutOfStock;
            }
            else if (Item.Quantity <= 5)
            {
                Item.ItemStatus = Status.RunningLow;
            }
            else
            {
                Item.ItemStatus = Status.Available;
            }

            if (Item.ItemID == 0)
            {
                await _db.AddItemAsync(Item);
            }
            else
            {
                await _db.UpdateItemAsync(Item);
            }
        }
    }
}