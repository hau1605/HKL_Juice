using HKL_Juice.Models;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Script.Serialization;
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
            Get("/admin/login", parameters => {
                return View["loginAdmin.cshtml"];
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
              
                return Response.AsJson(productsWithCategories);
            }
            );

            Get("/admin/order", (parameters) =>
             {
                 var orders = dbContext.Order
           .Select(o => new
           {
               orderId = o.orderId,
               orderDate = o.orderDate,
               paymentStatus = o.paymentStatus,
               orderTotal = o.orderTotal,
               userFullname = o.User.userFullname,
               userId = o.User.userId,
               paymentMethod=o.paymentMethod,
               OrderDetails = o.OrderDetails.Select(od => new 
               {
                   productName = od.Product.productName,
                   price = od.Product.price,
                   productId = od.productId,
                   quantity = od.quantity,
                   subTotal = od.subTotal,
                   imgUrl = od.Product.imgUrl
               }).ToList()
           }).ToList();
                 var serializer = new JavaScriptSerializer();
                 string json = serializer.Serialize(orders);
                 return View["orderAdmin.cshtml", json];
             });

             

        }
    }
}