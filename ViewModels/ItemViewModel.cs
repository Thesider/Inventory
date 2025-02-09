using Inventory.Data;
using Inventory.Logger;
using Inventory.Models;
using Inventory.ViewModels.Interface;
using System.Collections.ObjectModel;

namespace Inventory.ViewModels;

public class ItemViewModel : IItemViewModel
{
    private readonly DatabaseService _db;
    private readonly IAppLogger _logger;
    public event Action? OnItemsChanged;


    public ObservableCollection<Items> Items { get; } = new();
    private List<Items> _allItems = new();
    public List<Items> FilteredItems { get; private set; } = new();

    public ItemViewModel(DatabaseService db, IAppLogger logger)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    private void UpdateCollections(Items item)
    {
        var existingItem = Items.FirstOrDefault(i => i.ItemID == item.ItemID);
        if (existingItem != null)
        {
            var index = Items.IndexOf(existingItem);
            Items[index] = item;

            var filteredIndex = FilteredItems.FindIndex(i => i.ItemID == item.ItemID);
            if (filteredIndex != -1)
            {
                FilteredItems[filteredIndex] = item;
            }

            var allItemsIndex = _allItems.FindIndex(i => i.ItemID == item.ItemID);
            if (allItemsIndex != -1)
            {
                _allItems[allItemsIndex] = item;
            }
        }
    }
    public async Task LoadItems()
    {
        try
        {
            _logger.LogInformation("Starting to load items.");
            var items = await _db.GetItemsAsync();
            _allItems = items.ToList();
            FilteredItems = _allItems.ToList();
            Items.Clear();
            foreach (var item in _allItems)
            {
                Items.Add(item);
            }
            _logger.LogInformation($"Finished loading items. Total items loaded: {_allItems.Count}");
            OnItemsChanged?.Invoke();

        }
        catch (Exception ex)
        {
            _logger.LogError("Error loading items", ex);
            throw;
        }
    }
    public async Task OnItemChanged(Items item)
    {
        try
        {
            await _db.UpdateItemAsync(item);
            _logger.LogInformation($"Updated item: {item.ItemName} (ID: {item.ItemID})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to update item: {item.ItemName}", ex);
        }
    }


    public async Task DeleteItem(int id)
    {
        var item = Items.FirstOrDefault(i => i.ItemID == id);
        if (item != null)
        {
            await _db.DeleteItemAsync(item);
            OnItemsChanged?.Invoke();

        }
    }

    public async Task AddItemAsync(Items item)
    {
        try
        {
            await _db.AddItemAsync(item);
            Items.Add(item);
            FilteredItems.Add(item);
            _logger.LogInformation($"Added item: {item.ItemName} (ID: {item.ItemID})");
            OnItemsChanged?.Invoke();

        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to add item: {item.ItemName}", ex);
        }
    }

    public async Task UpdateItemAsync(Items item)
    {
        try
        {
            await _db.UpdateItemAsync(item);
            _logger.LogInformation($"Updated item: {item.ItemName} (ID: {item.ItemID})");
            OnItemsChanged?.Invoke();

        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to update item: {item.ItemName}", ex);
        }
    }


    public async Task DeleteItemAsync(Items item)
    {
        try
        {
            await _db.DeleteItemAsync(item);
            Items.Remove(item);
            FilteredItems.Remove(item);
            _logger.LogInformation($"Deleted item: {item.ItemName} (ID: {item.ItemID})");
            OnItemsChanged?.Invoke();

        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to delete item: {item.ItemName}", ex);
        }
    }
    public async Task RefreshAsync()
    {
        await LoadItems();
    }

    public async Task RefreshItemAsync(Items item)
    {
        try
        {
            await _db.RefreshItemAsync(item);
            _logger.LogInformation($"Refreshed item: {item.ItemName} (ID: {item.ItemID})");
            OnItemsChanged?.Invoke();

        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to refresh item: {item.ItemName}", ex);
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
    public void SortItems(string sortBy, bool ascending)
    {
        try
        {
            _logger.LogInformation($"Sorting items by {sortBy} in {(ascending ? "ascending" : "descending")} order");

            FilteredItems = sortBy.ToLower() switch
            {
                "itemname" => ascending
                    ? FilteredItems.OrderBy(i => i.ItemName).ToList()
                    : FilteredItems.OrderByDescending(i => i.ItemName).ToList(),
                "quantity" => ascending
                    ? FilteredItems.OrderBy(i => i.Quantity).ToList()
                    : FilteredItems.OrderByDescending(i => i.Quantity).ToList(),
                "expiredate" => ascending
                    ? FilteredItems.OrderBy(i => i.ExpireDate).ToList()
                    : FilteredItems.OrderByDescending(i => i.ExpireDate).ToList(),
                "itemid" => ascending
                    ? FilteredItems.OrderBy(i => i.ItemID).ToList()
                    : FilteredItems.OrderByDescending(i => i.ItemID).ToList(),
                "wholesaleprice" => ascending
                    ? FilteredItems.OrderBy(i => i.WholesalePrice).ToList()
                    : FilteredItems.OrderByDescending(i => i.WholesalePrice).ToList(),
                "retailprice" => ascending
                    ? FilteredItems.OrderBy(i => i.RetailPrice).ToList()
                    : FilteredItems.OrderByDescending(i => i.RetailPrice).ToList(),
                "itemcategory" => ascending
                    ? FilteredItems.OrderBy(i => i.ItemCategory).ToList()
                    : FilteredItems.OrderByDescending(i => i.ItemCategory).ToList(),
                "manufacturer" => ascending
                    ? FilteredItems.OrderBy(i => i.Manufacturer).ToList()
                    : FilteredItems.OrderByDescending(i => i.Manufacturer).ToList(),
                "origin" => ascending
                    ? FilteredItems.OrderBy(i => i.Origin).ToList()
                    : FilteredItems.OrderByDescending(i => i.Origin).ToList(),
                "itemstatus" => ascending
                    ? FilteredItems.OrderBy(i => GetItemStatus(i)).ToList()
                    : FilteredItems.OrderByDescending(i => GetItemStatus(i)).ToList(),
                _ => FilteredItems.OrderBy(i => i.ItemName).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sorting items by {sortBy}", ex);
            throw;
        }
    }
    public void FilterItems(string filterText, string filterColumn)
    {
        try
        {
            _logger.LogInformation($"Filtering items by {filterColumn} with text: {filterText}");

            if (string.IsNullOrWhiteSpace(filterText))
            {
                FilteredItems = _allItems.ToList();
                OnItemsChanged?.Invoke();
                return;
            }

            var lowerFilterText = filterText.ToLowerInvariant();

            FilteredItems = filterColumn.ToLower() switch
            {
                "name" => _allItems.AsParallel().Where(i => i.ItemName != null && i.ItemName.ToLowerInvariant().Contains(lowerFilterText)).ToList(),
                "wholesaleprice" => _allItems.AsParallel().Where(i => i.WholesalePrice.ToString().Contains(filterText)).ToList(),
                "retailprice" => _allItems.AsParallel().Where(i => i.RetailPrice.ToString().Contains(filterText)).ToList(),
                "category" => _allItems.AsParallel().Where(i => i.ItemCategory.ToString().ToLowerInvariant().Contains(lowerFilterText)).ToList(),
                "manufacturer" => _allItems.AsParallel().Where(i => i.Manufacturer != null && i.Manufacturer.ToLowerInvariant().Contains(lowerFilterText)).ToList(),
                "origin" => _allItems.AsParallel().Where(i => i.Origin != null && i.Origin.ToLowerInvariant().Contains(lowerFilterText)).ToList(),
                _ => _allItems.AsParallel().Where(i => i.ItemName != null && i.ItemName.ToLowerInvariant().Contains(lowerFilterText)).ToList()
            };

            OnItemsChanged?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error filtering items by {filterColumn}", ex);
            throw;
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
