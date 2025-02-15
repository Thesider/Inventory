using Inventory.Data;
using Inventory.Models;
using Inventory.ViewModels.Interface;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Inventory.ViewModels
{
    public class ServiceViewModel : IServiceViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<ServiceViewModel> _logger;

        public ObservableCollection<Services> Services { get; private set; } = new();

        public ServiceViewModel(DatabaseService databaseService, ILogger<ServiceViewModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task LoadServices()
        {
            try
            {
                _logger.LogInformation("Loading services...");
                var services = await _databaseService.GetServicesAsync();
                Services.Clear();
                foreach (var service in services)
                {
                    Services.Add(service);
                }
                _logger.LogInformation("Services loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load services.", ex);
            }
        }

        public async Task AddService(Services service)
        {
            try
            {
                await _databaseService.AddServiceAsync(service);
                Services.Add(service);
                _logger.LogInformation($"Service added: {service.ServiceName}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to add service.", ex);
            }
        }

        public async Task UpdateService(Services service)
        {
            try
            {
                await _databaseService.UpdateServiceAsync(service);
                var existingService = Services.FirstOrDefault(s => s.ServiceId == service.ServiceId);
                if (existingService != null)
                {
                    var index = Services.IndexOf(existingService);
                    Services[index] = service;
                }
                _logger.LogInformation($"Service updated: {service.ServiceName}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update service.", ex);
            }
        }

        public async Task DeleteService(int serviceId)
        {
            try
            {
                await _databaseService.DeleteServiceAsync(serviceId);
                var service = Services.FirstOrDefault(s => s.ServiceId == serviceId);
                if (service != null)
                {
                    Services.Remove(service);
                }
                _logger.LogInformation($"Service deleted: {serviceId}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete service.", ex);
            }
        }


    }
}
