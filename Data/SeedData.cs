using Inventory.Enums;
using Inventory.Models;

namespace Inventory.Data;

public class SeedData
{
    private readonly DatabaseService _databaseService;

    public SeedData(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task SeedDatabaseAsync()
    {
        var items = new List<Items>
    {
        new Items { ItemName = "Item 1", Origin = "USA", Manufacturer = "Manufacturer 1", Quantity = 10, RetailPrice = 15999, WholesalePrice = 10999, ExpireDate = DateTime.Today.AddMonths(6), ItemCategory = Category.Pill },
        new Items { ItemName = "Item 2", Origin = "Canada", Manufacturer = "Manufacturer 2", Quantity = 20, RetailPrice = 25999, WholesalePrice = 20999, ExpireDate = DateTime.Today.AddMonths(12), ItemCategory = Category.Syrup },
        new Items { ItemName = "Item 3", Origin = "Germany", Manufacturer = "Manufacturer 3", Quantity = 30, RetailPrice = 35999, WholesalePrice = 30999, ExpireDate = DateTime.Today.AddMonths(18), ItemCategory = Category.Injection },
        new Items { ItemName = "Item 4", Origin = "France", Manufacturer = "Manufacturer 4", Quantity = 40, RetailPrice = 45999, WholesalePrice = 40999, ExpireDate = DateTime.Today.AddMonths(24), ItemCategory = Category.Cream },
        new Items { ItemName = "Item 5", Origin = "Japan", Manufacturer = "Manufacturer 5", Quantity = 50, RetailPrice = 55999, WholesalePrice = 50999, ExpireDate = DateTime.Today.AddMonths(30), ItemCategory = Category.Powder }
    };
        var users = new List<Users>
    {
        new Users { UserName = "admin", Password = "admin" ,Name ="admin" }
    };
        var services = new List<Services>
    {
        new Services { ServiceName = "Service 1", ServicePrice = 1000 },
        new Services { ServiceName = "Service 2", ServicePrice = 2000 },
        new Services { ServiceName = "Service 3", ServicePrice = 3000 },
        new Services { ServiceName = "Service 4", ServicePrice = 4000 },
        new Services { ServiceName = "Service 5", ServicePrice = 5000 }
    };

        foreach (var item in items)
        {
            await _databaseService.SaveItemAsync(new List<Items> { item });
        }
        foreach (var service in services)
        {
            await _databaseService.AddServiceAsync(service);
        }
    }

}