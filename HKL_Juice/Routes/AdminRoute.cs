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


    public class ExcelProduct
    {
        public List<Product> excelProducts { get; set; }
    }
    public class AdminRoute : NancyModule
    {
        public  AdminRoute(ApplicationDbContext dbContext)
        {
            Get("/admin", parameters => {
                var now = DateTime.Now;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var totalRevenueThisMonth = dbContext.Order
                    .Where(o => o.paymentStatus == "Đã thanh toán" && 
                    o.orderDate >= firstDayOfMonth 
                    && o.orderDate <= now)
                    .Sum(o => o.orderTotal);
                var numberOfProductsOnSale = dbContext.Product.Count();

                var numberOfNewOrdersThisMonth = dbContext.Order
                    .Count(o => o.paymentStatus == "Đã thanh toán" && 
                    o.orderDate >= firstDayOfMonth && o.orderDate <= now);
                var orderCountsLast7Days = new List<object>();
                var revenueLast14Days = new List<object>();
                for (int i = 0; i <= 6; i++)
                {
                    var date = now.AddDays(-i).Date;
                    var dateString = date.ToString("dd-MM-yyyy");
                    var orderCount = dbContext.Order
                        .Count(o => o.paymentStatus == "Đã thanh toán" &&
                                    o.orderDate.Year == date.Year &&
                                    o.orderDate.Month == date.Month &&
                                    o.orderDate.Day == date.Day);
                    orderCountsLast7Days.Add(new
                    {
                        Date = dateString,
                        OrderCount = orderCount
                    });
                }
                // Prepare the data to be sent to the view
                for (int i = 0; i <= 13; i++)
                {
                    var date = now.AddDays(-i).Date;
                    var dateString = date.ToString("dd-MM-yyyy");
                    // Calculating revenue
                    var dailyRevenue = dbContext.Order
                        .Where(o => o.paymentStatus == "Đã thanh toán" &&
                                    o.orderDate.Year == date.Year &&
                                    o.orderDate.Month == date.Month &&
                                    o.orderDate.Day == date.Day)
                        .Sum(o => (decimal?)o.orderTotal) ?? 0;
                    revenueLast14Days.Add(new
                    {
                        Date = dateString,
                        DailyRevenue = dailyRevenue
                    });
                }

                var data = new
                {
                    TotalRevenueThisMonth = totalRevenueThisMonth,
                    NumberOfProductsOnSale = numberOfProductsOnSale,
                    NumberOfNewOrdersThisMonth = numberOfNewOrdersThisMonth,
                    OrderCountsLast7Days = orderCountsLast7Days,
                    RevenueLast14Days = revenueLast14Days
                };

                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(data);
                return View["admin.cshtml", json];
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
                if (orders.Count > 0)
                    return HttpStatusCode.BadRequest;
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
                var cateGory = dbContext.Category.Select(p=>new
                {
                    categoryId = p.categoryId,
                    categoryName = p.categoryName
                }).ToList();
                var productData = new
                {
                    products = productsWithCategories,
                    cateGory = cateGory
                };
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(productData);
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
            Post("/admin/excelproducts", parameters =>
            {
                try
                {
                    var excelData = this.Bind<ExcelProduct>();
                    if (excelData == null || excelData.excelProducts == null)
                    {
                        // Log the issue or return an error response
                        Console.WriteLine("excelData or excelData.excelProducts is null.");
                        return HttpStatusCode.BadRequest;
                    }

                    foreach (var excelProduct in excelData.excelProducts)
                    {
                        dbContext.Product.Add(excelProduct);

                        dbContext.SaveChanges();
                    }

                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    // Log exception details here
                    Console.WriteLine(ex.ToString());
                    return HttpStatusCode.InternalServerError;
                }
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
                if (orderDetails.Count>0)
                    return HttpStatusCode.BadRequest;
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

            // Account
            Get("/admin/account", parameters =>
            {
                return View["accountAdmin.cshtml"];
            });
            Put("/admin/account/{userId}", parameters =>
            {
                int userId = parameters.userId;
                var user = dbContext.User.FirstOrDefault(u => u.userId == userId);
                if (user == null)
                {
                    return HttpStatusCode.NotFound;
                }
                var putUser = this.Bind<User>();
                user.userFullname = putUser.userFullname;
                user.userPhone = putUser.userPhone;
                user.userAvatar = putUser.userAvatar;
                if (!string.IsNullOrEmpty(putUser.userPassword))
                {
                    user.userPassword = putUser.userPassword;
                }

                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });

            // Settings === Category
            Get("/admin/settings", parameters =>
            {
                var categories = dbContext.Category
                                        .Select(c => new
                                        {
                                            categoryId = c.categoryId,
                                            categoryName = c.categoryName,
                                            numsProduct = dbContext.Product.Count(p => p.categoryId == c.categoryId)
                                        }).ToList();

                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(categories);
                return View["setting.cshtml", json];
            });
            Post("/admin/categories", parameters =>
            {
                var postCategory = this.Bind<Category>();

                dbContext.Category.Add(postCategory);
                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });
            Put("/admin/categories/{id}", parameters =>
            {
                int categoryId = parameters.id;
                var catrgory = dbContext.Category.FirstOrDefault(c => c.categoryId == categoryId);
                if (catrgory == null)
                {
                    return HttpStatusCode.NotFound;
                }
                var putCategory = this.Bind<Category>();
                catrgory.categoryName = putCategory.categoryName;
                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });
            Delete("/admin/categories/{id}", parameters =>
            {

                int categoryId = parameters.id;
                var products = dbContext.Product.Where(p => p.categoryId == categoryId).ToList();
                if (products.Count > 0)
                {
                    return HttpStatusCode.BadRequest;
                }

                var category = dbContext.Category.FirstOrDefault(c => c.categoryId == categoryId);
                if (category == null)
                {
                    return HttpStatusCode.NotFound;
                }
                dbContext.Category.Remove(category);
                dbContext.SaveChanges();

                return HttpStatusCode.OK;
            });
        }
    }
}