using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.Models
{
    public class OrderProduct
    {
        public OrderProduct()
        {
            this.NetPrice = 0;
            this.Quantity = 1;
        }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal NetPrice { get; set; }
        
        public decimal CalculatedPrice => this.Quantity * this.NetPrice;
    }
}
