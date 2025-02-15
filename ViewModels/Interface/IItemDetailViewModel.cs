using Inventory.Models;

namespace Inventory.ViewModels.Interface
{
    public interface IItemDetailViewModel
    {
        Items Item { get; }
        Task LoadItem(int id);
        void NewItem();
        Task SaveItem();
    }
}
