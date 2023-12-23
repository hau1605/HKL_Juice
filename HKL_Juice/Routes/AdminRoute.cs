using HKL_Juice.Models;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace HKL_Juice.Routes
{
    public class AdminRoute : NancyModule
    {
        public  AdminRoute(ApplicationDbContext dbContext)
        {
            Get("/admin", parameters => {
                return View["admin.cshtml"];
            }
            );

            Get("/admin/product", parameters => {
                var productsWithCategories = dbContext.Product
                                        .Include(p => p.Category)
                                        .Select(p => new
                                        {
                                            ProductName = p.productName,
                                            CategoryName = p.Category.categoryName
                                        }).ToList();

                foreach (var item in productsWithCategories)
                {
                    Console.WriteLine($"Product: {item.ProductName}, Category: {item.CategoryName}");
                }
                return Response.AsJson(productsWithCategories);
            }
           );
             Get("/admin/order",( parameters) => {
                var orders =  dbContext.Order
                 .Include("User") // Including Product navigation property in OrderDetail
                .ToList();
                 return Response.AsJson(orders);
            }
           );

        }
    }
}