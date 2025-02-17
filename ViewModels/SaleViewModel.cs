using Inventory.Data;
using Inventory.Enums;
using Inventory.Models;
using Inventory.ViewModels.Interface;
using Microsoft.Extensions.Logging;

public class SaleViewModel : ISaleViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly ILogger<SaleViewModel> _logger;

    public List<Items> Items { get; private set; } = new();
    public List<Items> FilteredItems { get; private set; } = new();
    public string SearchQuery { get; set; } = string.Empty;
    public Dictionary<string, InventorySummary> InventorySummary { get; private set; } = new();
    public List<Sales> SalesHistory { get; private set; } = new();
    public List<DailySaleReport> DailySaleReport { get; private set; } = new();
    public List<MonthlySalesReport> MonthlySalesReport { get; private set; } = new();
    public SaleFormModel SaleFormModel { get; private set; } = new SaleFormModel();

    public SaleViewModel(DatabaseService databaseService, ILogger<SaleViewModel> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    public async Task LoadSaleData()
    {
        _logger.LogInformation("Loading sale data");
        var loadDataTask = LoadData();
        var loadSalesHistoryTask = LoadSalesHistory();
        var generateDailySaleReportTask = GenerateDailySaleReport();
        var generateMonthlySalesReportTask = GenerateMonthlySalesReport();

        await Task.WhenAll(loadDataTask, loadSalesHistoryTask, generateDailySaleReportTask, generateMonthlySalesReportTask);
        FilteredItems = Items.Where(it => it.Quantity > 0).ToList();
    }
    private async Task LoadData()
    {
        try
        {
            Items = await _databaseService.GetItemsAsync();

            InventorySummary.Clear();
            foreach (var category in Enum.GetValues(typeof(Category)).Cast<Category>())
            {
                var categoryItems = Items.Where(i => i.ItemCategory == category).ToList();
                InventorySummary[category.ToString()] = new InventorySummary
                {
                    TotalItems = categoryItems.Count,
                    InStock = categoryItems.Count(i => i.Quantity > i.CriticalAmmount),
                    LowStock = categoryItems.Count(i => i.Quantity > 0 && i.Quantity <= i.CriticalAmmount),
                    OutOfStock = categoryItems.Count(i => i.Quantity == 0),
                    Expiring = categoryItems.Count(i => i.ExpireDate <= DateTime.Today.AddDays(30)),
                    Expired = categoryItems.Count(i => i.ExpireDate <= DateTime.Today)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load data");
        }
    }

    private async Task LoadSalesHistory()
    {
        try
        {
            SalesHistory = await _databaseService.GetSalesHistoryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load sales history");
        }
    }

    private async Task GenerateDailySaleReport()
    {
        try
        {
            var salesHistory = await _databaseService.GetSalesHistoryAsync();

            // Group by date and then by transaction
            DailySaleReport = salesHistory
                .GroupBy(s => s.SaleDate.Date)
                .Select(dateGroup => new DailySaleReport
                {
                    Date = dateGroup.Key,
                    Revenue = dateGroup.Sum(s => s.TotalAmount + (s.AddOnAmount ?? 0)),
                    SaleItems = dateGroup
                        .Select(sale => new SaleItemDetail
                        {
                            ItemName = sale.ItemName ?? string.Empty,
                            Quantity = sale.QuantitySold,
                            Amount = sale.TotalAmount,
                            AddOnName = sale.AddOnName ?? string.Empty,
                            AddOnAmount = sale.AddOnAmount ?? 0
                        })
                        .ToList()
                })
                .OrderByDescending(r => r.Date)
                .ToList();

            _logger.LogInformation("Daily sales report generated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate daily sales report");
        }
    }

    private async Task GenerateMonthlySalesReport()
    {
        try
        {
            var salesHistory = await _databaseService.GetSalesHistoryAsync();
            MonthlySalesReport = salesHistory
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new MonthlySalesReport
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalQuantitySold = g.Sum(s => s.QuantitySold),
                    TotalRevenue = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(r => r.Month)
                .ToList();
            _logger.LogInformation("Monthly sales report generated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate monthly sales report");
        }
    }

    public async Task<bool> ProcessSale()
    {
        if (!SaleFormModel.SaleItems.Any())
        {
            _logger.LogWarning("No sale items added.");
            return false;
        }

        foreach (var saleItem in SaleFormModel.SaleItems)
        {
            if (saleItem.ItemId == 0 || saleItem.Quantity <= 0)
            {
                _logger.LogWarning("Invalid sale item entry. Please check that an item is selected and quantity is greater than 0.");
                return false;
            }
        }

        var groupedSale = new GroupedSale
        {
            SaleDate = DateTime.Now,
            SaleItems = new List<SaleItem>(),
            TotalAmount = 0
        };

        try
        {
            foreach (var saleItem in SaleFormModel.SaleItems)
            {
                var selectedItem = Items.FirstOrDefault(i => i.ItemID == saleItem.ItemId);
                if (selectedItem == null)
                {
                    _logger.LogWarning($"Selected item with ID {saleItem.ItemId} not found.");
                    return false;
                }
                if (selectedItem.Quantity < saleItem.Quantity)
                {
                    _logger.LogWarning($"Insufficient stock for item {selectedItem.ItemName}. Requested: {saleItem.Quantity}, Available: {selectedItem.Quantity}");
                    return false;
                }

                selectedItem.Quantity -= saleItem.Quantity;

                var totalAmount = saleItem.Quantity * selectedItem.RetailPrice;
                groupedSale.TotalAmount += totalAmount;

                groupedSale.SaleItems.Add(new SaleItem
                {
                    ItemId = saleItem.ItemId,
                    Quantity = saleItem.Quantity,
                    AddOnName = saleItem.AddOnName,
                    AddOnAmount = saleItem.AddOnAmount
                });

                var saleRecord = new Sales
                {
                    ItemName = selectedItem.ItemName,
                    QuantitySold = saleItem.Quantity,
                    SaleDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    AddOnName = saleItem.AddOnName,
                    AddOnAmount = saleItem.AddOnAmount,
                    SaleUnit = SaleFormModel.Unit
                };

                await _databaseService.SaveSaleAsync(saleRecord);

                _logger.LogInformation($"Prepared sale for {saleItem.Quantity} unit(s) of {selectedItem.ItemName}.");
            }

            var itemsToUpdate = SaleFormModel.SaleItems
                .Select(si => Items.FirstOrDefault(i => i.ItemID == si.ItemId))
                .Where(item => item != null)
                .Distinct()
                .Cast<Items>()
                .ToList();

            await _databaseService.SaveItemAsync(itemsToUpdate);

            await _databaseService.SaveGroupedSaleAsync(groupedSale);

            _logger.LogInformation("Successfully processed multi-item sale.");

            await LoadSaleData();

            SaleFormModel = new SaleFormModel();
            SaleFormModel.SaleItems.Add(new SaleItem());

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process multi-item sale.");
            return false;
        }
    }

    public async Task UpdateSaleAmount(Sales sale)
    {
        try
        {
            await _databaseService.UpdateSaleAsync(sale);
            _logger.LogInformation($"Updated sale amount for {sale.ItemName} to {sale.TotalAmount}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update sale amount.");
        }
    }

    public void AddSaleItem()
    {
        SaleFormModel.SaleItems.Add(new SaleItem());
    }

    public void RemoveSaleItem(SaleItem saleItem)
    {
        SaleFormModel.SaleItems.Remove(saleItem);
    }
    public void SearchItems(string searchQuery)
    {
        SearchQuery = searchQuery?.Trim().ToLower() ?? string.Empty;

        FilteredItems = string.IsNullOrWhiteSpace(SearchQuery)
            ? Items.Where(it => it.Quantity > 0).ToList()
            : Items.Where(it => it.Quantity > 0 &&
                (it.ItemName.ToLower().Contains(SearchQuery) ||
                 it.ItemCategory.ToString().ToLower().Contains(SearchQuery)))
                .ToList();
    }


    public void AddAddOn(SaleItem saleItem, string addOnName, double addOnAmount)
    {
        saleItem.AddOnName = addOnName;
        saleItem.AddOnAmount = addOnAmount;
    }

    public void RemoveAddOn(SaleItem saleItem)
    {
        saleItem.AddOnName = null;
        saleItem.AddOnAmount = null;
    }
}
