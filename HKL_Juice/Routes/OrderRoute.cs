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
            Get("/api/v1/orders/{numberTable}", parameters =>
            {
                try
                {
                    int numberTable = parameters.numberTable;
                    var orders = dbContext.Order
                        .Where(o => o.numberTable == numberTable &&
                                    (o.paymentStatus == "Đang chờ" || o.paymentStatus == "Đã thanh toán"))
                        .OrderByDescending(o => o.orderDate)
                        .Select(o => new
                        {
                            paymentStatus = o.paymentStatus,
                            orderTotal = o.orderTotal,
                            orderStatus = o.orderStatus,
                            OrderDetails = o.OrderDetails.Select(od => new
                            {
                                productId = od.productId,
                                productName = od.Product.productName,
                                price = od.Product.price,
                                quantity = od.quantity,
                                subTotal = od.subTotal,
                                imgUrl = od.Product.imgUrl
                            }).ToList()
                        }).ToList();

                    return Response.AsJson(orders);
                } catch
                {
                    return HttpStatusCode.BadRequest;
                }
            });
            Get("/api/v1/order-details", parameters =>
            {
                try
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
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
            });
            Post("/api/v1/order", (parameters) =>
            {
                try
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
                } catch
                {
                    return HttpStatusCode.BadRequest;
                }
            });

            Post("/api/v1/order/{numberTable}", (parameters) =>
            {
                try
                {
                    int numberTable = parameters.numberTable;
                    var existingOrder = dbContext.Order
                        .Where(o => o.numberTable == numberTable && o.paymentStatus == "Đang chờ")
                        .OrderByDescending(o => o.orderDate)
                        .FirstOrDefault();
                    if (existingOrder != null) // Cập nhật
                    {
                        var updateOrder = this.Bind<CreateOrder>();
                        existingOrder.orderDate = DateTime.Now;
                        existingOrder.orderTotal += updateOrder.orderTotal;
                        existingOrder.orderStatus = "Đơn mới";

                        /*var updatedOrder = dbContext.Order.Add(existingOrder);*/
                        var updatedOrder = existingOrder;
                        dbContext.SaveChanges();
                        var orderId = existingOrder.orderId;

                        foreach (var detail in updateOrder.orderDetail)
                        {
                            var updateDetail = new OrderDetail
                            {
                                orderId = orderId,
                                productId = detail.productId,
                                quantity = detail.quantity,
                                subTotal = detail.subTotal,
                                isNew = true
                            };
                            var updatedDetail = dbContext.OrderDetail.Add(updateDetail);
                            dbContext.SaveChanges();
                        }
                        var sdas11 = updateOrder;
                        return Response.AsJson(new
                        {
                            Success = true,
                            Message = "Cập nhật"
                        });
                    }
                    else // Update
                    {
                        var createOrder = this.Bind<CreateOrder>();
                        var order = new Order()
                        {
                            userId = 1,
                            orderDate = DateTime.Now,
                            paymentMethod = "Tiền mặt",
                            orderStatus = "Đơn mới",
                            paymentStatus = createOrder.paymentStatus,
                            orderTotal = createOrder.orderTotal,
                            numberTable = numberTable,
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
                                subTotal = detail.subTotal,
                                isNew = true
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
                } catch
                {
                    return HttpStatusCode.BadRequest;
                }
            });
        }
    }
}