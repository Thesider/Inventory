using Inventory.Models;

namespace Inventory.ViewModels.Interface
{
    public interface ISaleViewModel
    {
        List<Items> Items { get; }
        List<Items> FilteredItems { get; } // Add this property
        Dictionary<string, InventorySummary> InventorySummary { get; }
        string SearchQuery { get; set; }
        List<Sales> SalesHistory { get; }
        List<DailySaleReport> DailySaleReport { get; }
        List<MonthlySalesReport> MonthlySalesReport { get; }
        SaleFormModel SaleFormModel { get; }
        Task LoadSaleData();
        Task UpdateSaleAmount(Sales sale);
        Task<bool> ProcessSale();
        void AddSaleItem();
        void RemoveSaleItem(SaleItem saleItem);
        void AddAddOn(SaleItem saleItem, string addOnName, double addOnAmount);
        void RemoveAddOn(SaleItem saleItem);
        void SearchItems(string searchQuery); // Add this method
    }
}
