using Inventory.Enum;
using SQLite;

namespace Inventory.Models
{
    [Table("Users")]
    public class Users
    {
        [AutoIncrement, PrimaryKey]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime LastLogin { get; set; }

        public Role UserRole { get; set; }



    }
}
