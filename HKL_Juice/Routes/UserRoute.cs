using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKL_Juice.Routes
{
    public class UserRoute : NancyModule
    {
        public UserRoute(ApplicationDbContext dbContext)
        {
            Get("/user", parameters =>
            {


                return View["juice.cshtml"];
            }
            );
        }
    }
}