using SQLite;

namespace Inventory.Models
{
    [Table("SaleItem")]
    public class SaleItem
    {
        [PrimaryKey, AutoIncrement]
        public int SaleId { get; set; }

        [Column("ItemId")]
        public int ItemId { get; set; }
        [Column("Quantity")]
        public int Quantity { get; set; } = 1;
        [Column("AddOnName")]
        public string? AddOnName { get; set; }
        [Column("AddOnAmount")]
        public double? AddOnAmount { get; set; } = 0;
        public string ItemSearchQuery { get; set; } = string.Empty;

    }

}
