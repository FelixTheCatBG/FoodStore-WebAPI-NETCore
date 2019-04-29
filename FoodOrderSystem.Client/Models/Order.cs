using FoodOrderSystem.Client.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.Models
{
    public class Order
    {
        public Order()
        {
            this.TotalPrice = 0;
            this.TakeAway = false;
            this.OrderTime = DateTime.Now;
            this.ReservationTime = DateTime.Now;
            this.OrderStatus = OrderStatus.Accepted;
        }

        public int Id { get; set; }

        public DateTime OrderTime { get; }

        public DateTime ReservationTime { get; set; }

        public decimal TotalPrice { get; set; } 

        public bool TakeAway { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();
    }

}
