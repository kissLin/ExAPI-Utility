﻿using System.Web.Mvc;

namespace NetFrameworkApp.Areas.V2
{
    public class V2AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "V2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "V2_default",
                "V2/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}