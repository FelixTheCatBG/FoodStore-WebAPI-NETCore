using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.DTO
{
    public class MakeOrderRootObjectDTO
    {
        [Required(ErrorMessage = "Order Information is required")]
        public OrderDTO Info { get; set; }

        [Required(ErrorMessage = "List of product and their quantity is required")]
        public List<MakeOrderProductDTO> Products { get; set; }
    }
}
