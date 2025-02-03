using Inventory.Models;
using SQLite;
using System.Diagnostics;

namespace Inventory.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        private List<Items>? _cachedItems;
        private DateTime _cacheTimestamp;
        public event Action? OnDataChanged;

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
                Debug.WriteLine($"File deletion failed: {ex.Message}");
            }
#endif

            _database = new SQLiteAsyncConnection(databasePath, Constants.Flags);
        }

        public async Task InitializeAsync()
        {
            await InitializeDatabase().ConfigureAwait(false);
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
                await _database.CreateTableAsync<Items>().ConfigureAwait(false);
                Debug.WriteLine("Table 'Items' created successfully.");

                Debug.WriteLine($"Attempting to create table 'Sales' at path: {Constants.DatabasePath}");
                await _database.CreateTableAsync<Sales>().ConfigureAwait(false);
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

        // Items Section
        public async Task<List<Items>> GetItemsAsync()
        {
            if (_cachedItems == null || DateTime.Now - _cacheTimestamp > TimeSpan.FromMinutes(5))
            {
                _cachedItems = await _database.Table<Items>()
                                              .ToListAsync()
                                              .ConfigureAwait(false);
                _cacheTimestamp = DateTime.Now;
            }
            return _cachedItems!;
        }

        public Task<Items?> GetItemAsync(int id)
        {
            return _database.Table<Items>()
                            .Where(i => i.ItemID == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(List<Items> items)
        {
            int result = 0;
            try
            {
                await _database.RunInTransactionAsync(tran =>
                {
                    foreach (var item in items)
                    {
                        result += item.ItemID != 0
                            ? tran.Update(item)
                            : tran.Insert(item);
                    }
                }).ConfigureAwait(false);

                NotifyDataChanged();
                _cachedItems = null;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw new InvalidOperationException("Transaction failed.", ex);
            }
            return result;
        }

        public async Task<int> AddItemAsync(Items item)
        {
            var result = await _database.InsertAsync(item).ConfigureAwait(false);
            _cachedItems = null;
            NotifyDataChanged();
            OnDataChanged?.Invoke();
            return result;
        }

        public async Task<int> UpdateItemAsync(Items item)
        {
            var result = await _database.UpdateAsync(item).ConfigureAwait(false);
            _cachedItems = null;
            NotifyDataChanged();
            OnDataChanged?.Invoke();
            return result;
        }

        public async Task<int> DeleteItemAsync(Items item)
        {
            try
            {
                if (item.ItemID <= 0)
                {
                    Debug.WriteLine("Delete failed: Invalid ItemID");
                    return 0;
                }

                var result = await _database.DeleteAsync<Items>(item.ItemID).ConfigureAwait(false);
                _cachedItems = null;
                Debug.WriteLine($"Deleted {result} row(s) for ID {item.ItemID}");
                NotifyDataChanged();
                OnDataChanged?.Invoke();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete error: {ex.Message}");
                throw;
            }
        }

        public async Task RefreshItemAsync(Items item)
        {
            var existingItem = await _database.FindAsync<Items>(item.ItemID).ConfigureAwait(false);
            if (existingItem != null)
            {
                item.ItemName = existingItem.ItemName;
                item.Origin = existingItem.Origin;
                item.Manufacturer = existingItem.Manufacturer;
                item.Quantity = existingItem.Quantity;
                item.RetailPrice = existingItem.RetailPrice;
                item.WholesalePrice = existingItem.WholesalePrice;
                item.ExpireDate = existingItem.ExpireDate;
                item.ItemCategoryInt = existingItem.ItemCategoryInt;
                item.ItemStatusInt = existingItem.ItemStatusInt;
            }
        }

        private async Task<bool> IsItemNameUniqueAsync(string itemName, int itemId = 0)
        {
            var existingItem = await _database.Table<Items>()
                                              .Where(i => i.ItemName == itemName && i.ItemID != itemId)
                                              .FirstOrDefaultAsync()
                                              .ConfigureAwait(false);
            return existingItem == null;
        }

        // Sales Section
        public Task<List<Sales>> GetSalesAsync()
        {
            return _database.Table<Sales>()
                            .ToListAsync();
        }

        public async Task SaveSaleAsync(Sales sale)
        {
            await _database.InsertAsync(sale).ConfigureAwait(false);
        }

        public Task<int> DeleteSaleAsync(Sales sale)
        {
            return _database.DeleteAsync(sale);
        }

        public async Task<List<Sales>> GetSalesHistoryAsync(int skip = 0, int take = 100)
        {
            return await _database.Table<Sales>()
                                  .OrderBy(s => s.SaleID)
                                  .Skip(skip)
                                  .Take(take)
                                  .ToListAsync()
                                  .ConfigureAwait(false);
        }

        private void NotifyDataChanged()
        {
            OnDataChanged?.Invoke();
        }
    }
}