using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ProductIngredient> ProductIngredients { get; set; } = new HashSet<ProductIngredient>();
    }
}
