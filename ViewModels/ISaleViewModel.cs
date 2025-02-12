using Inventory.Models;

namespace Inventory.ViewModels
{
    public interface ISaleViewModel
    {
        List<Items> Items { get; }
        Dictionary<string, InventorySummary> InventorySummary { get; }
        List<Sales> SalesHistory { get; }
        List<DailySaleReport> DailySaleReport { get; }
        List<MonthlySalesReport> MonthlySalesReport { get; }
        SaleFormModel SaleFormModel { get; }
        Task LoadSaleData();
        Task<bool> ProcessSale();
        void AddSaleItem();
        void RemoveSaleItem(SaleItem saleItem);
        void AddAddOn(SaleItem saleItem, string addOnName, double addOnAmount);
        void RemoveAddOn(SaleItem saleItem);
    }
}
