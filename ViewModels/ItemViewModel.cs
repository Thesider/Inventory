using Inventory.Data;
using Inventory.Models;
using System.Collections.ObjectModel;

namespace Inventory.ViewModels;

public class ItemViewModel
{
	
    private readonly DatabaseService _db;

    public ObservableCollection<Items> Items { get; } = new();

    public ItemViewModel(DatabaseService db)
    {
        _db = db;
    }

    public async Task LoadItems()
    {
        var items = await _db.GetItemsAsync();
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public async Task DeleteItem(int id)
    {
        var item = Items.FirstOrDefault(i => i.ItemID == id);
        if (item != null)
        {
            await _db.DeleteItemAsync(item);
        }
    }
}