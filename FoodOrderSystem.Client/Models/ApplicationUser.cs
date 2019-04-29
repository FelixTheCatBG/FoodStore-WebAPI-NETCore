using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base() {
          
        }

        //Will be implemented later
        //public string FirstName { get; set; }

        //public string LastName { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();


    }
}
