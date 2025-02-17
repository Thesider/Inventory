namespace Inventory.Models
{
    public class DailySaleReport
    {
        public DateTime Date { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int TotalQuantitySold { get; set; }
        public double Revenue { get; set; }
        public List<SaleItemDetail> SaleItems { get; set; } = new();

    }
}