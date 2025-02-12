using Inventory.Enums;

namespace Inventory.Models
{
    public class SaleFormModel
    {
        public List<SaleItem> SaleItems { get; set; } = new();
        public Unit Unit { get; set; }

    }
}
