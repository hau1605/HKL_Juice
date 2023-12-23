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
              
                return Response.AsJson(productsWithCategories);
            }
           );
             Get("/admin/order",( parameters) => {
                 var orders = dbContext.Order
            .Select(o => new
            {
                userName = o.User.userFullname,
                OrderDetailList = o.OrderDetails.Select(od => new
                {
                    OrderDetailId = od.orderDetailId,
                    ProductId = od.productId,
                    Quantity = od.quantity,
                    Subtotal = od.subTotal,
                    imgUrl=od.Product.imgUrl
                }).ToList()
            }).ToList();

                 return Response.AsJson(orders);
             }
           );

        }
    }
}