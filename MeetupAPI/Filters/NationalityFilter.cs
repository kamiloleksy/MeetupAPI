using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Filters
{
    public class NationalityFilter : Attribute, IAuthorizationFilter
    {
        private string[] _nationalities;
        public NationalityFilter(string nationalities)
        {
            _nationalities = nationalities.Split(",");
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var nationality = context.HttpContext.User.FindFirst(c => c.Type == "Nationality").Value;

            if(!_nationalities.Any(c=>c == nationality))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
