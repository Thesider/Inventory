using Inventory.Models;

namespace Inventory.ViewModels
{
    public interface IItemDetailViewModel
    {
        Items Item { get; }
        Task LoadItem(int id);
        void NewItem();
        Task SaveItem();
    }
}
