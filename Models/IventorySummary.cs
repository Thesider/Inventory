namespace Inventory.Models
{
    public class InventorySummary
    {
        public int TotalItems { get; set; }
        public int InStock { get; set; }
        public int LowStock { get; set; }
        public int Expired { get; set; }
    }

}
