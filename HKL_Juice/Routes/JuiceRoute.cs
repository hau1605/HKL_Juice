using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKL_Juice.Routes
{
    public class JuiceRoute : NancyModule
    {
        public JuiceRoute(ApplicationDbContext dbContext)
        {
            Get("/juice", parameters =>
            {


                return View["juice.cshtml"];
            }
            );
        }
    }
}