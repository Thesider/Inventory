using SQLite;

namespace Inventory.Models
{
    [Table("Sales")]
    public class Sales
    {
        [AutoIncrement]
        [PrimaryKey]
        public int SaleID { get; set; }

        [Column("ItemID")]
        public int ItemID { get; set; }

        [Column("ItemName")]
        public string? ItemName { get; set; }

        [Column("QuantitySold")]
        public int QuantitySold { get; set; }

        [Column("SaleDate")]
        public DateTime SaleDate { get; set; }

        [Column("TotalAmount")]
        public double TotalAmount { get; set; }
        [Ignore]
        public Unit SaleUnit { get; set; }
        public enum Unit
        {
            Pharmacy, Clinic, Personal, Other
        }


    }
}