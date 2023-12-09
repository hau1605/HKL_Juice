using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKL_Juice.Routes
{
    public class ContactRoute : NancyModule
    {
        public ContactRoute(ApplicationDbContext dbContext)
        {
            Get("/contact", parameters =>
            {


                return View["contact.cshtml"];
            }
            );
        }
    }
}