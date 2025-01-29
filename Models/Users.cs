using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class Users
    {
        public int UserID {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name {  get; set; }
       
        public enum Role { Admin, Store , Clinic}
        public Role UserRole  { get;set; }



    }
}
