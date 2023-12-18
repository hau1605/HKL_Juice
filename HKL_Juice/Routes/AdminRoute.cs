using HKL_Juice.Models;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKL_Juice.Routes
{
    public class AdminRoute : NancyModule
    {
        public AdminRoute(ApplicationDbContext dbContext)
        {
            Get("/admin", parameters => {
                return View["admin.cshtml"];
            }
            );

        }
    }
}