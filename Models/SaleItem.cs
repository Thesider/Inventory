namespace Inventory.Models
{
    public class SaleItem
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public double? SpecialPrice { get; set; }


        public double? SpecialPrice { get; set; }
        public List<AddOn> AddOns { get; set; } = new List<AddOn>();


    }
}