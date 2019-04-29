using FoodOrderSystem.Client.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.Data
{
    public class Initializer
    {
        public static async Task Initialize(
            FoodOrderIdentityContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            //Ensures the database is deleted and created everytime we run the program
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var categories = new[]
            {          
                 new Category() { Name = "Pizzas"},
                 new Category() {Name = "Drinks"},
                 new Category() {Name = "Burgers"},
                 new Category() {Name = "HotDogs"},

            };
            //Seeding database with data to work with
            var products = new[]
            {
                 new Product() {Name = "Vegetarianna", Price = 40,Category = categories[0]},
                 new Product() {Name = "Rafaelo", Price = 50, Category = categories[0]},
                 new Product() {Name = "Capricciosa", Price = 60,Category = categories[0]},
                 new Product() {Name = "Speciale", Price = 70, Category = categories[0]},
                 new Product() {Name = "Juice", Price = 20,Category= categories[1]},
                 new Product() {Name = "Coca-Cola", Price = 16,Category=categories[1]},
                 new Product() {Name = "Sprite", Price = 14,Category=categories[1]},
                 new Product() {Name = "Water", Price = 10,Category=categories[1]},
                 new Product() {Name = "Chicken Burger", Price = 35,Category= categories[2]},
                 new Product() {Name = "Hot Chicken", Price = 40,Category= categories[2]},
                 new Product() {Name = "Bacon Burger", Price = 35,Category= categories[2]},
                 new Product() {Name = "Salami Burger", Price = 30,Category= categories[2]},
                 new Product() {Name = "Small Hotdog", Price = 10,Category= categories[3]},
                 new Product() {Name = "Medium Hotdog", Price = 15,Category= categories[3]},
                 new Product() {Name = "Big Hotdog", Price = 20,Category= categories[3]},
                 new Product() {Name = "Special Hotdog", Price = 25,Category= categories[3]}
            };
            context.Products.AddRange(products);
            context.SaveChanges();

            //Seeding with Ingredients
            var ingredients = new[]
            {
                 new Ingredient() {Name = "Tomatoes"},
                 new Ingredient() {Name = "Peppers"},
                 new Ingredient() {Name = "Cheese"},
            };
            context.Ingredients.AddRange(ingredients);
            context.SaveChanges();

            var productIngredients = new[]
            {
                new ProductIngredient(){ Product = products[0], Ingredient = ingredients[0]},
                new ProductIngredient(){ Product = products[0], Ingredient = ingredients[1]},
                new ProductIngredient(){ Product = products[0], Ingredient = ingredients[2]},
                new ProductIngredient(){ Product = products[1], Ingredient = ingredients[0]},
                new ProductIngredient(){ Product = products[1], Ingredient = ingredients[1]},
                new ProductIngredient(){ Product = products[1], Ingredient = ingredients[2]},
                new ProductIngredient(){ Product = products[2], Ingredient = ingredients[0]},
                new ProductIngredient(){ Product = products[2], Ingredient = ingredients[1]},
                new ProductIngredient(){ Product = products[2], Ingredient = ingredients[2]},
                new ProductIngredient(){ Product = products[3], Ingredient = ingredients[0]},
                new ProductIngredient(){ Product = products[3], Ingredient = ingredients[1]},
                new ProductIngredient(){ Product = products[3], Ingredient = ingredients[2]}
            };
            context.ProductIngredients.AddRange(productIngredients);
            context.SaveChanges();

            //Seeding with admin and first user
            string password = "password";

            if (await userManager.FindByNameAsync("admin@email.com") == null)
            {

                var user = new ApplicationUser
                {
                    UserName = "admin@email.com",
                    Email = "admin@email.com",
                    PhoneNumber = "333444555"
                };

                var result = await userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            if (await userManager.FindByNameAsync("first@email.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "first@email.com",
                    Email = "first@email.com",
                    PhoneNumber = "333444555"
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Customer");
                }

                var orders = new[]
                {
                    new Order
                    {
                        ReservationTime = new DateTime(2018,11,10),
                        TotalPrice = 22,
                        TakeAway = true,
                        ApplicationUser = user
                    },
                    new Order
                    {
                        ReservationTime = new DateTime(2018,11,9),
                        TotalPrice = 25,
                        TakeAway = true,
                        ApplicationUser = user
                    }
                 };

                context.Orders.AddRange(orders);
                context.SaveChanges();

                var orderProducts = new[]
                {
                new OrderProduct(){ Order = orders[0], Product = products[0], Quantity=2,NetPrice = 40},
                new OrderProduct(){ Order = orders[0], Product = products[1], Quantity=4,NetPrice = 30},
                new OrderProduct(){ Order = orders[0], Product = products[2], Quantity=5,NetPrice = 20}
                };
                context.OrderProducts.AddRange(orderProducts);
                context.SaveChanges();
            }

            //Seeding with discounts for products
            var discounts = new[]
            {
                 new Discount() { DiscountedPrice=20 ,ProductId=1},
                 new Discount() { DiscountedPrice=10 ,ProductId=4},
                 new Discount() { DiscountedPrice=5 ,ProductId=6}

            };
            context.Discounts.AddRange(discounts);
            context.SaveChanges();
        }

    }
}
