using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.DTO
{
    public class OrderDTO
    {
        [Required(ErrorMessage = "Reservation time is required")]
        public DateTime ReservationTime { get; set; }

        [Required(ErrorMessage = "Total price is required")]
        public int TotalPrice { get; set; }

        [Required(ErrorMessage = "Take-away option is required")]
        public bool TakeAway { get; set; }

        public string Email { get; set; } // this property is for the Showcase controller only & will be deleted


    }
}
