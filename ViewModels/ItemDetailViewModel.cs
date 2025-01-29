using Inventory.Data;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class ItemDetailViewModel
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
                Item.ItemStatus = Items.Status.OutOfStock;
            }
            else if (Item.Quantity <= 5)
            {
                Item.ItemStatus = Items.Status.RunningLow;
            }
            else
            {
                Item.ItemStatus = Items.Status.Available;
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
