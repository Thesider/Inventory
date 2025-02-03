namespace Inventory.Models
{
    public class MonthlySalesReport
    {
        public DateTime Month { get; set; }
        public int TotalQuantitySold { get; set; }
        public double TotalRevenue { get; set; }
    }
}
