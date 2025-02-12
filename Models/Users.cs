
using Inventory.Enums;

namespace Inventory.Models
{
    public class Users
    {
        public int UserID { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }


        public Role UserRole { get; set; }



    }
}
