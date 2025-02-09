using Inventory.Data;
using Inventory.Enum;
using Inventory.Models;
using Inventory.ViewModels.Interface;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

public class SaleViewModel : ISaleViewModel, INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private readonly ILogger<SaleViewModel> _logger;
    public List<Items> Items { get; private set; } = new();
    public Dictionary<string, InventorySummary> InventorySummary { get; private set; } = new();
    public List<Sales> SalesHistory { get; private set; } = new();
    public List<DailySaleReport> DailySalesReport { get; private set; } = new();
    public List<MonthlySalesReport> MonthlySalesReport { get; private set; } = new();
    public SaleFormModel SaleFormModel { get; private set; } = new SaleFormModel();

    public event PropertyChangedEventHandler? PropertyChanged;

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
        var generateMonthlySalesReportTask = GenerateMonthlySalesReport();
        var generateDailySalesReportTask = GenerateDailySalesReport();

        await Task.WhenAll(loadDataTask, loadSalesHistoryTask, generateMonthlySalesReportTask);
        OnPropertyChanged(nameof(DailySalesReport));
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
                    InStock = categoryItems.Count(i => i.Quantity > 0),
                    LowStock = categoryItems.Count(i => i.Quantity > 0 && i.Quantity <= 5),
                    Expired = categoryItems.Count(i => i.ExpireDate <= DateTime.Today)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load data", ex);
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
            _logger.LogError("Failed to load sales history", ex);
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
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate monthly sales report", ex);
        }
    }
    private async Task GenerateDailySalesReport()
    {
        try
        {
            var salesHistory = await _databaseService.GetSalesHistoryAsync();
            DailySalesReport = salesHistory
                .Where(s => s.SaleUnit == Unit.Pharmacy && s.SaleDate.Date == DateTime.Today)
                .Select(s => new DailySaleReport
                {
                    SaleDate = s.SaleDate,
                    ItemName = s.ItemName,
                    Quantity = s.QuantitySold,
                    TotalAmount = s.TotalAmount
                })
                .OrderByDescending(r => r.SaleDate)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate daily sales report", ex);
        }
    }
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
    public async Task<bool> ProcessSale()
    {
        // Validate that there is at least one sale item
        if (!SaleFormModel.SaleItems.Any())
        {
            _logger.LogWarning("No sale items added.");
            return false;
        }

        // Validate the unit selection
        if (SaleFormModel.Unit == Unit.Other)
        {
            _logger.LogWarning("No unit selected.");
            return false;
        }

        // Validate each sale item entry
        foreach (var saleItem in SaleFormModel.SaleItems)
        {
            if (saleItem.ItemId == 0 || saleItem.Quantity <= 0)
            {
                _logger.LogWarning("Invalid sale item entry. Please check that an item is selected and quantity is greater than 0.");
                return false;
            }
        }

        // Prepare to process each sale item
        var saleRecords = new List<Sales>();
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

            // Deduct the sold quantity from the selected item
            selectedItem.Quantity -= saleItem.Quantity;

<<<<<<< Updated upstream
            // Apply different pricing formulas based on the sale unit
            double totalAmount = saleItem.Quantity * selectedItem.WholesalePrice * (saleItem.SpecialPrice ?? 1);

=======
            // Calculate the total amount including add-ons
            double addOnTotal = saleItem.AddOns.Sum(addOn => addOn.price ?? 0);
            double totalAmount = saleItem.Quantity * (selectedItem.WholesalePrice) * (saleItem.SpecialPrice ?? 1) + addOnTotal;
>>>>>>> Stashed changes

            // Create a sale record for the sale item
            var saleRecord = new Sales
            {
                ItemID = selectedItem.ItemID,
                ItemName = selectedItem.ItemName,
                QuantitySold = saleItem.Quantity,
                SaleDate = DateTime.Now,
                TotalAmount = totalAmount,
                SaleUnit = SaleFormModel.Unit
            };
            saleRecords.Add(saleRecord);

            _logger.LogInformation($"Prepared sale for {saleItem.Quantity} unit(s) of {selectedItem.ItemName}.");
        }

        try
        {
            // Save updated items
            var itemsToUpdate = SaleFormModel.SaleItems
                .Select(si => Items.FirstOrDefault(i => i.ItemID == si.ItemId))
                .Where(item => item != null)
                .Distinct()
                .ToList();

            await _databaseService.SaveItemAsync(itemsToUpdate!);

            // Save each sale record
            foreach (var saleRecord in saleRecords)
            {
                await _databaseService.SaveSaleAsync(saleRecord);
            }

            _logger.LogInformation("Successfully processed multi-item sale.");

            // Reload updated data
            var loadDataTask = LoadData();
            var loadSalesHistoryTask = LoadSalesHistory();
            var generateDailySalesReportTask = GenerateDailySalesReport();
            var generateMonthlySalesReportTask = GenerateMonthlySalesReport();

            await Task.WhenAll(loadDataTask, loadSalesHistoryTask, generateDailySalesReportTask, generateMonthlySalesReportTask);

            // Reset the sale form
            SaleFormModel = new SaleFormModel();
            SaleFormModel.SaleItems.Add(new SaleItem());

            OnPropertyChanged(nameof(DailySalesReport));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to process multi-item sale.", ex);
            return false;
        }
    }
    public void AddSaleItem()
    {
        SaleFormModel.SaleItems.Add(new SaleItem());
        OnPropertyChanged(nameof(SaleFormModel));
    }

    public void RemoveSaleItem(SaleItem saleItem)
    {
        SaleFormModel.SaleItems.Remove(saleItem);
    }
}

    public void UpdateAvailableItems(int selectedItemId)
    {
        var selectedItem = Items.FirstOrDefault(i => i.ItemID == selectedItemId);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
