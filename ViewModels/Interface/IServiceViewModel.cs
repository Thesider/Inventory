using Inventory.Models;
using System.Collections.ObjectModel;

namespace Inventory.ViewModels.Interface
{
    public interface IServiceViewModel
    {
        ObservableCollection<Services> Services { get; }

        Task LoadServices();
        Task AddService(Services service);
        Task UpdateService(Services service);
        Task DeleteService(int serviceId);
    }
}
