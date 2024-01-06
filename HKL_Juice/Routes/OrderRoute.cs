using HKL_Juice.Models;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Script.Serialization;
using Nancy.ModelBinding;
using System.Web.Services.Description;

namespace HKL_Juice.Routes
{

    public class CreateOrder
    {
        public int orderTotal { get; set; }
        public int numberTable { get; set; }
        public string paymentStatus { get; set; }
        public List<CreateOrderDetail> orderDetail { get; set; }
    }
    public class CreateOrderDetail
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public int subTotal { get; set; }
    }
    public class OrderRoute : NancyModule
    {
        public OrderRoute(ApplicationDbContext dbContext)
        {
            Get("/api/v1/orders", parameters =>
            {
                var orders = dbContext.Order
                                  .Select(o => new
                                  {
                                      orderId = o.orderId,
                                      orderDate = o.orderDate,
                                      paymentStatus = o.paymentStatus,
                                      userId = o.userId,
                                      paymentMethod = o.paymentMethod,
                                      orderTotal = o.orderTotal,
                                      numberTable = o.numberTable
                                  }).ToList();
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(orders);
                return Response.AsJson(orders);
            });
            Get("/api/v1/order-details", parameters =>
            {
                var orderDetails = dbContext.OrderDetail
                                  .Select(od => new
                                  {
                                      orderDetailId = od.orderDetailId,
                                      orderId = od.orderId,
                                      productId = od.productId,
                                      quantity = od.quantity,
                                      subTotal = od.subTotal
                                  }).ToList();
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(orderDetails);
                return Response.AsJson(orderDetails);
            });
            Post("/api/v1/order", (parameters) =>
            {
                var createOrder = this.Bind<CreateOrder>();
                var order = new Order()
                {
                    orderDate = DateTime.Now,
                    paymentStatus = createOrder.paymentStatus,
                    userId = 1,
                    paymentMethod = "Tiền mặt",
                    orderTotal = createOrder.orderTotal,
                    numberTable = createOrder.numberTable
                };
                var createdOrder = dbContext.Order.Add(order);
                dbContext.SaveChanges();
                var orderId = order.orderId;

                var orderDetail = createOrder.orderDetail;
                foreach (var detail in orderDetail)
                {
                    var newDetail = new OrderDetail
                    {
                        orderId = orderId,
                        productId = detail.productId,
                        quantity = detail.quantity,
                        subTotal = detail.subTotal
                    };
                    var createdOrderDetail = dbContext.OrderDetail.Add(newDetail);
                    dbContext.SaveChanges();
                }

                return HttpStatusCode.OK;
            });

            Post("/api/v1/order/{numberTable}", (parameters) =>
            {
                int numberTable = parameters.numberTable;

                var existingOrder = dbContext.Order
                    .Where(o => o.numberTable == numberTable && o.paymentStatus == "Đang chờ")
                    .OrderByDescending(o => o.orderDate)
                    .FirstOrDefault();
                if (existingOrder != null) // Cập nhật
                {
                    var updateOrder = this.Bind<CreateOrder>();
                    existingOrder.orderTotal = updateOrder.orderTotal;
                    var updatedOrder = dbContext.Order.Add(existingOrder);
                    dbContext.SaveChanges();
                    var orderId = existingOrder.orderId;

                    foreach (var detail in updateOrder.orderDetail)
                    {
                        var newDetail = new OrderDetail
                        {
                            orderId = orderId,
                            productId = detail.productId,
                            quantity = detail.quantity,
                            subTotal = detail.subTotal
                        };
                        dbContext.OrderDetail.Add(newDetail);
                        dbContext.SaveChanges();
                    }
                    return Response.AsJson(new
                    {
                        Success = true,
                        Message = "Cập nhật",
                        invoice = updateOrder
                    });
                }
                else
                {
                    var createOrder = this.Bind<CreateOrder>();
                    var order = new Order()
                    {
                        orderDate = DateTime.Now,
                        paymentStatus = createOrder.paymentStatus,
                        userId = 1,
                        paymentMethod = "Tiền mặt",
                        orderTotal = createOrder.orderTotal,
                        numberTable = numberTable
                    };
                    var createdOrder = dbContext.Order.Add(order);
                    dbContext.SaveChanges();
                    var orderId = order.orderId;

                    var orderDetail = createOrder.orderDetail;
                    foreach (var detail in orderDetail)
                    {
                        var newDetail = new OrderDetail
                        {
                            orderId = orderId,
                            productId = detail.productId,
                            quantity = detail.quantity,
                            subTotal = detail.subTotal
                        };
                        var createdOrderDetail = dbContext.OrderDetail.Add(newDetail);
                        dbContext.SaveChanges();
                    }
                    return Response.AsJson(new 
                    { 
                        Success = true, 
                        Message = "Tạo mới",
                        invoice = createOrder 
                    });
                }
            });
        }
    }
}