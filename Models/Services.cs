using SQLite;

namespace Inventory.Models
{
    [Table("Services")]

    public class Services
    {
        [PrimaryKey, AutoIncrement]
        [Column("ServiceId")]
        public int ServiceId { get; set; }
        [Column("Service Name")]
        public string ServiceName { get; set; }
        [Column("Service Price")]
        public double ServicePrice { get; set; }
        [Column("Quantity")]
        public int Quantity { get; set; }

    }
}
