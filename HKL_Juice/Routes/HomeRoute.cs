using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data.Entity;

namespace HKL_Juice.Routes
{
    public class HomeRoute : NancyModule
    {
        public HomeRoute(ApplicationDbContext dbContext)
        {
            /*Get("/", parameters => {


                return View["index.cshtml"];
            }
            );*/
            Get("/", parameters => {

                var products = dbContext.Product
                   .Select(p => new
                   {
                       productId = p.productId,
                       categoryId = p.categoryId,
                       productName = p.productName,
                       price = p.price,
                       descript = p.descript,
                       imgUrl = p.imgUrl,
                       OrderDetails = p.Category.categoryName
                   }).ToList();

                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(products);

                return View["home.cshtml", json];
            });
        }
    }
}