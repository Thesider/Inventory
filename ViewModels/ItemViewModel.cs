using Inventory.Data;
using Inventory.Models;
using Inventory.Services.Logging;
using System.Collections.ObjectModel;
using System.Linq;

namespace Inventory.ViewModels;

public class ItemViewModel
{
    private readonly DatabaseService _db;
    private readonly IAppLogger _logger;

    public ObservableCollection<Items> Items { get; } = new();

    public ItemViewModel(DatabaseService db, IAppLogger logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task LoadItems()
    {
        try
        {
            var items = await _db.GetItemsAsync();
            Items.Clear();
            foreach (var item in items)
            {
                CheckInventoryStatus(item);
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load inventory items", ex);
        }
    }

    public async Task DeleteItem(int id)
    {
        var item = Items.FirstOrDefault(i => i.ItemID == id);
        if (item != null)
        {
            try
            {
                await _db.DeleteItemAsync(item);
                Items.Remove(item);
                _logger.LogInformation($"Deleted item: {item.ItemName} (ID: {item.ItemID})");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete item: {item.ItemName}", ex);
            }
        }
    }

    public async Task SaveItem(Items item)
    {
        try
        {
            await _db.SaveItemAsync(item);
            if (!Items.Contains(item))
            {
                Items.Add(item);
            }
            _logger.LogInformation($"Saved item: {item.ItemName} (ID: {item.ItemID})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save item: {item.ItemName}", ex);
        }
    }

    public void CheckInventoryStatus(Items item)
    {
        if (item.ExpireDate <= DateTime.Today)
        {
            _logger.LogWarning($"Item {item.ItemName} (ID: {item.ItemID}) has expired.");
        }
        else if (item.ExpireDate <= DateTime.Today.AddDays(30))
        {
            _logger.LogWarning($"Item {item.ItemName} (ID: {item.ItemID}) will expire soon.");
        }

        if (item.Quantity == 0)
        {
            _logger.LogWarning($"Item {item.ItemName} (ID: {item.ItemID}) is out of stock.");
        }
        else if (item.Quantity <= 5)
        {
            _logger.LogWarning($"Item {item.ItemName} (ID: {item.ItemID}) is running low.");
        }
    }

    public string GetItemStatus(Items item)
    {
        if (item.ExpireDate <= DateTime.Today)
            return "Expired";
        if (item.Quantity == 0)
            return "Out of Stock";
        if (item.Quantity <= 5)
            return "Low Stock";
        return "Available";
    }

    public string GetRowClass(Items item)
    {
        if (item.ExpireDate <= DateTime.Today)
            return "table-danger";
        if (item.Quantity == 0)
            return "table-warning";
        if (item.Quantity <= 5)
            return "table-info";
        return "";
    }
}
