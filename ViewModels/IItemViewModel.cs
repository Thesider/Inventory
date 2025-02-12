using Inventory.Models;
using System.Collections.ObjectModel;

namespace Inventory.ViewModels
{
    public interface IItemViewModel
    {
        ObservableCollection<Items> Items { get; }
        List<Items> FilteredItems { get; }
        event Action? OnItemsChanged;

        Task LoadItems();
        Task OnItemChanged(Items item);
        Task DeleteItem(int id);
        Task AddItemAsync(Items item);
        Task UpdateItemAsync(Items item);
        Task DeleteItemAsync(Items item);
        Task RefreshAsync();
        Task RefreshItemAsync(Items item);
        void CheckInventoryStatus(Items item);
        void SortItems(string sortBy, bool ascending);
        void CheckAllInventoryStatus();

        void FilterItems(string filterText, string filterColumn);
        string GetItemStatus(Items item);

        string GetRowClass(Items item);
    }
}
