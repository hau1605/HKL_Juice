using HKL_Juice.Models;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Script.Serialization;
using Nancy.ModelBinding;

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


            //Dang nhap
            Post("/admin/loginUser", parameters =>
            {
                var userLogin = this.Bind<LoginRequest>();
                var userName = userLogin.userName;
                var userPassword = userLogin.userPassword;
                // Thực hiện kiểm tra đăng nhập tại đây
                var user = dbContext.User
                             .FirstOrDefault(u => u.userName == userName && u.userPassword == userPassword);
                if (user != null)
                {
                    // Người dùng hợp lệ
                    return Response.AsJson(new
                    {
                        Success = true,
                        Message = "Đăng nhập thành công.",
                        UserData = new
                        {
                            user.userId,
                            user.userName,
                            // Do not return sensitive data like passwords
                            user.userFullname,
                            user.userPhone,
                            user.Role.roleName
                        }
                    });
                }
                else
                {
                    // Đăng nhập thất bại
                    return Response
                        .WithHeader("Access-Control-Allow-Origin", "*")
                        .WithHeader("Access-Control-Allow-Methods", "POST,GET,PUT,DELETE,OPTIONS")
                        .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type, Authorization")
                        .AsJson(new { Success = false, Message = "Tên đăng nhập hoặc mật khẩu không đúng." });
                }
            });


        }
    }
}