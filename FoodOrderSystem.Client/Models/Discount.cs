using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.Models
{
    public class Discount
    {     
        public int Id { get; set; }

        public decimal DiscountedPrice { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}
