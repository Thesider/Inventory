using Inventory.Models;
using SQLite;
using System.Diagnostics;

namespace Inventory.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "inventory.db");
            _database = new SQLiteAsyncConnection(databasePath);
            _database.ExecuteAsync("DROP TABLE IF EXISTS Items").Wait();

            _database.CreateTableAsync<Items>().Wait();
        
        }

        public Task<List<Items>> GetItemsAsync()
        {
            return _database.Table<Items>().ToListAsync();
        }
        public Task<Items> GetItemAsync(int id)
        {
            return _database.Table<Items>()
                            .Where(i => i.ItemID == id)
                            .FirstOrDefaultAsync();
        }
        public async Task<int> SaveItemAsync(Items item)
        {
            if (!await IsItemNameUniqueAsync(item.ItemName, item.ItemID))
            {
                return -1; 
            }

            if (item.ItemID != 0)
            {
                return await _database.UpdateAsync(item);
            }
            else
            {
                return await _database.InsertAsync(item);
            }
        }
        public Task<int> AddItemAsync(Items item)
        {
            return _database.InsertAsync(item);
        }

        public Task<int> UpdateItemAsync(Items item)
        {
            return _database.UpdateAsync(item);
        }

        public Task<int> DeleteItemAsync(Items item)
        {
            return _database.DeleteAsync(item);
        }
        private async Task<bool> IsItemNameUniqueAsync(string itemName, int itemId = 0)
        {
            var existingItem = await _database.Table<Items>()
                                              .Where(i => i.ItemName == itemName && i.ItemID != itemId)
                                              .FirstOrDefaultAsync();
            return existingItem == null;
        }
    }
}