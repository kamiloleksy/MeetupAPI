using MeetupAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetupAPI.Authorization
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRecuirement>
    {
        public ILogger<MinimumAgeHandler> _logger { get; }
        public MinimumAgeHandler(ILogger<MinimumAgeHandler> logger)
        {
            _logger = logger;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRecuirement requirement)
        {
           var userEmail = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
           var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);

            _logger.LogInformation($"Handling minimum age requirement to {userEmail}. [dateOfBirth: {dateOfBirth}]");

            if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
            {
                _logger.LogInformation("Access granted");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation("Access denied");
            }

            return Task.CompletedTask;
        }

    }
}
