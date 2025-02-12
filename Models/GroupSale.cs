using SQLite;

namespace Inventory.Models
{
    [Table("GroupedSale")]
    public class GroupedSale
    {
        [PrimaryKey, AutoIncrement]
        public int SaleId { get; set; }

        [Column("SaleDate")]
        public DateTime SaleDate { get; set; }

        [Column("TotalAmount")]
        public double TotalAmount { get; set; }

        [Ignore]
        public List<SaleItem> SaleItems { get; set; } = new();
    }
}
