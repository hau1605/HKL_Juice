using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKL_Juice.Routes
{
    public class CartRoute : NancyModule
    {
        public CartRoute(ApplicationDbContext dbContext)
        {
            Get("/cart", parameters =>
            {


                return View["cart.cshtml"];
            }
            );
        }
    }
}