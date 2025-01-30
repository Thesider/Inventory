using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    [Table("Items")]
    public class Items
    {
        [AutoIncrement]
        [Column("ID"), PrimaryKey]
        public int ItemID { get; set; }

        [Column("Name")]
        [Unique]
        [Required(ErrorMessage = "Item Name is required")]
        public string ItemName { get; set; }

        [Column("Origin")]
        public string Origin { get; set; }

        [Column("Manufacturer")]
        public string Manufacturer { get; set; }

        [Column("Quantity")]
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Column("Retail")]
        public double RetailPrice { get; set; }
        [Column("Wholesale")]
        public double WholesalePrice { get; set; }

        [Column("ExpireDate")]
        public DateTime ExpireDate { get; set; }

        [Column("ItemCategory")]
        public int ItemCategoryInt
        {
            get => (int)ItemCategory;
            set => ItemCategory = (Category)value;
        }

        [Ignore]
        public Category ItemCategory { get; set; }


        [Column("ItemStatus")]
        public int ItemStatusInt
        {
            get => (int)ItemStatus;
            set => ItemStatus = (Status)value;
        }

        [Ignore]
        public Status ItemStatus { get; set; }

        public enum Category
        {
            Pill, Syrup, Injection, Cream, Powder, Drops, Spray, Inhaler, Other
        }

        public enum Status
        {
            Available, OutOfStock, Expired, RunningLow
        }

        public enum SaleUnit
        {
          Store, Personal ,Clinic 

        }
    }
}
