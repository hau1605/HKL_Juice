using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKL_Juice.Routes
{
    public class AboutRoute : NancyModule
    {
        public AboutRoute(ApplicationDbContext dbContext)
        {
            Get("/about", parameters =>
            {

                return View["about.cshtml"];
            });
        }
    }
}