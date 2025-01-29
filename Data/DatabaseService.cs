using Inventory.Models;
using SQLite;
using System.Diagnostics;

namespace Inventory.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        private List<Items> _cachedItems;

        public DatabaseService()
        {
            var databasePath = Constants.DatabasePath;
#if DEBUG
            try
            {
                if (File.Exists(databasePath))
                {
                    File.Delete(databasePath);
                }
            }
            catch (IOException ex)
            {

                Console.WriteLine($"File deletion failed: {ex.Message}");
            }
#endif
            _database = new SQLiteAsyncConnection(databasePath, Constants.Flags);
        }

        public async Task InitializeAsync()
        {
            await InitializeDatabase();
        }
        private void LogError(Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }

        private async Task InitializeDatabase()
        {
            try
            {
                Debug.WriteLine($"Attempting to create table 'Items' at path: {Constants.DatabasePath}");
                await _database.CreateTableAsync(typeof(Items));
                Debug.WriteLine("Table 'Items' created successfully.");

                Debug.WriteLine($"Attempting to create table 'Sales' at path: {Constants.DatabasePath}");
                await _database.CreateTableAsync(typeof(Sales));
                Debug.WriteLine("Table 'Sales' created successfully.");

                Debug.WriteLine("Database initialized successfully.");
            }
            catch (SQLiteException sqlEx)
            {
                LogError(sqlEx);
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        public async Task<List<Items>> GetItemsAsync()
        {
            if (_cachedItems == null)
            {
                _cachedItems = await _database.Table<Items>().ToListAsync();
            }
            return _cachedItems;
        }

        public Task<Items> GetItemAsync(int id)
        {
            return _database.Table<Items>()
                            .Where(i => i.ItemID == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(List<Items> items)
        {
            int result = 0;
            await _database.RunInTransactionAsync(tran =>
            {
                foreach (var item in items)
                {
                    if (item.ItemID != 0)
                    {
                        result += tran.Update(item);
                    }
                    else
                    {
                        result += tran.Insert(item);
                    }
                }
            });
            return result;

        }
        //Item session of the DatabaseService class
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
        //sales session of the DatabaseService class
        public Task<List<Sales>> GetSalesAsync()
        {
            return _database.Table<Sales>().ToListAsync();
        }
        public async Task SaveSaleAsync(Sales sale)
        {
            await _database.InsertAsync(sale);
        }
        public Task<int> DeleteSaleAsync(Sales sale)
        {
            return _database.DeleteAsync(sale);
        }

        public async Task<List<Sales>> GetSalesHistoryAsync()
        {
            return await _database.Table<Sales>().ToListAsync();
        }




    }
}
