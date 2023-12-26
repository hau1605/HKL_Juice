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
    public class LoginRequest
    {
        public string userName { get; set; }
        public string userPassword { get; set; }
    }
    public class PutOrder
    {
        public string paymentMethod { get; set; }
        public string paymentStatus { get; set; }
        public int userId { get; set; }
    }
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
            

            Get("/admin/users", parameters => {
                var resUser = dbContext.User
                                        .Select(u => new
                                        {
                                            userId = u.userId,
                                            userFullname = u.userFullname,
                                            userPhone = u.userPhone,
                                            userName = u.userName,
                                            roleId = u.roleId,
                                            roleName=u.Role.roleName,
                                            userAvatar = u.userAvatar,
                                        }).ToList();
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(resUser);
                return View["userAdmin.cshtml", json];
            }
           );
            Post("/admin/users", parameters =>
            {
                var postUser = this.Bind<User>();
                postUser.userAvatar = "/Content/assets/images/user.png";
                var existingUser = dbContext.User.FirstOrDefault(u => u.userName == postUser.userName);
                if (existingUser != null)
                {
                    // Username already exists, return bad request
                    return HttpStatusCode.BadRequest;
                }
                dbContext.User.Add(postUser);
                dbContext.SaveChanges();
                return HttpStatusCode.OK;
            });
            Put("/admin/users/{id}", parameters =>
            {
                int userId = parameters.id;
                var user = dbContext.User.FirstOrDefault(u => u.userId == userId);
                if (user == null)
                {
                    return HttpStatusCode.NotFound;
                }
                var putUser = this.Bind<User>();
              

                user.roleId = putUser.roleId;
                user.userFullname = putUser.userFullname;
                user.userPhone = putUser.userPhone;
                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });
            Delete("/admin/users/{id}", parameters =>
            {

                int userId = parameters.id;
                var orders = dbContext.Order.Where(o => o.userId == userId).ToList();


                // Xóa tất cả các chi tiết hóa đơn liên quan
                foreach (var order in orders)
                {
                    dbContext.Order.Remove(order);
                }
                var user = dbContext.User.FirstOrDefault(u => u.userId == userId);
                if (user == null)
                {
                    return HttpStatusCode.NotFound;
                }
                dbContext.User.Remove(user);

                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });

            Get("/admin/products", parameters => {
                var productsWithCategories = dbContext.Product
                                        .Include(p => p.Category)
                                        .Select(p => new
                                        {
                                            productId = p.productId,
                                            categoryId = p.categoryId,
                                            productName = p.productName,
                                            price = p.price,
                                            descript = p.descript,
                                            imgUrl = p.imgUrl,
                                            categoryName = p.Category.categoryName
                                        }).ToList();
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(productsWithCategories);
                return View["productAdmin.cshtml", json];
            }
            );
            Post("/admin/products", parameters =>
            {
                
                var postProduct = this.Bind<Product>();

                dbContext.Product.Add(postProduct);
                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });
            Put("/admin/products/{id}", parameters =>
            {
                int productId = parameters.id;
                var product = dbContext.Product.FirstOrDefault(p => p.productId == productId);
                if (product == null)
                {
                    return HttpStatusCode.NotFound;
                }
                var putProduct = this.Bind<Product>();
                product.categoryId = putProduct.categoryId;
                product.productName = putProduct.productName;
                product.price = putProduct.price;
                product.descript = putProduct.descript;
                product.imgUrl = putProduct.imgUrl;
                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });
            Delete("/admin/products/{id}", parameters =>
            {

                int productId = parameters.id;
                var orderDetails = dbContext.OrderDetail.Where(od => od.productId == productId).ToList();


                // Xóa tất cả các chi tiết hóa đơn liên quan
                foreach (var detail in orderDetails)
                {
                    dbContext.OrderDetail.Remove(detail);
                }
                var product = dbContext.Product.FirstOrDefault(p => p.productId == productId);
                if (product == null)
                {
                    return HttpStatusCode.NotFound;
                }
                dbContext.Product.Remove(product);

                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });


            Get("/admin/order", parameters =>
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
              paymentMethod = o.paymentMethod,
              numberTable = o.numberTable,
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
            Post("/admin/order", parameters =>
            {
                var order = new Order
                {
                    orderDate = DateTime.Now,
                    paymentStatus = "Pending", // Hoặc bất kỳ trạng thái nào bạn muốn đặt mặc định
                    userId = 2,
                    paymentMethod = "faef",
                    orderTotal = 120000
                };

                // Thêm hóa đơn vào cơ sở dữ liệu
                dbContext.Order.Add(order);
                dbContext.SaveChanges();
                return Response.AsJson(order);
            });      
            Put("/admin/order/{id}", parameters =>
            {
                int orderId = parameters.id;
                var order = dbContext.Order.FirstOrDefault(o => o.orderId == orderId);
                if (order == null)
                {
                    return HttpStatusCode.NotFound;
                }
                var putOrder = this.Bind<PutOrder>();
                order.paymentMethod = putOrder.paymentMethod;
                order.paymentStatus = putOrder.paymentStatus;
                order.userId = putOrder.userId;

                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });
            Delete("/admin/order/{id}", parameters =>
            {

                int orderId = parameters.id;
                var orderDetails = dbContext.OrderDetail.Where(od => od.orderId == orderId).ToList();


                // Xóa tất cả các chi tiết hóa đơn liên quan
                foreach (var detail in orderDetails)
                {
                    dbContext.OrderDetail.Remove(detail);
                }
                var order = dbContext.Order.FirstOrDefault(o => o.orderId == orderId);
                if (order == null)
                {
                    return HttpStatusCode.NotFound;
                }
                dbContext.Order.Remove(order);

                dbContext.SaveChanges();

                return HttpStatusCode.OK;
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
                            user.userFullname,
                            user.userPhone,
                            user.Role.roleName,
                            user.userAvatar
                        }
                    });
                }
                else
                {
                    // Đăng nhập thất bại
                    return Response.AsJson(new { Success = false, Message = "Tên đăng nhập hoặc mật khẩu không đúng." });
                }
            });

            Get("/admin/account", parameters =>
            {
                return View["userAdmin.cshtml"];
            });
        }
    }
}