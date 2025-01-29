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
            new Items { ItemName = "Item 1", Origin = "USA", Manufacturer = "Manufacturer 1", Quantity = 10, RetailPrice = 15.99, WholesalePrice = 10.99, ExpireDate = DateTime.Today.AddMonths(6), ItemCategory = Items.Category.Pill },
            new Items { ItemName = "Item 2", Origin = "Canada", Manufacturer = "Manufacturer 2", Quantity = 20, RetailPrice = 25.99, WholesalePrice = 20.99, ExpireDate = DateTime.Today.AddMonths(12), ItemCategory = Items.Category.Syrup },
            new Items { ItemName = "Item 3", Origin = "Germany", Manufacturer = "Manufacturer 3", Quantity = 30, RetailPrice = 35.99, WholesalePrice = 30.99, ExpireDate = DateTime.Today.AddMonths(18), ItemCategory = Items.Category.Injection },
            new Items { ItemName = "Item 4", Origin = "France", Manufacturer = "Manufacturer 4", Quantity = 40, RetailPrice = 45.99, WholesalePrice = 40.99, ExpireDate = DateTime.Today.AddMonths(24), ItemCategory = Items.Category.Cream },
            new Items { ItemName = "Item 5", Origin = "Japan", Manufacturer = "Manufacturer 5", Quantity = 50, RetailPrice = 55.99, WholesalePrice = 50.99, ExpireDate = DateTime.Today.AddMonths(30), ItemCategory = Items.Category.Powder }
        };

        foreach (var item in items)
        {
            await _databaseService.SaveItemAsync(new List<Items> { item });
        }
    }
}