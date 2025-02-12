using Inventory.Enums;
using SQLite;

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

    [Column("AddOnName")]
    public string? AddOnName { get; set; }

    [Column("AddOnAmount")]
    public double? AddOnAmount { get; set; }

    [Column("SaleUnit")]
    public Unit SaleUnit { get; set; }
}
