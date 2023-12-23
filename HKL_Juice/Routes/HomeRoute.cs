using HKL_Juice.Models;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

                return View["home.cshtml"];
            }
            );
        }
    }
}