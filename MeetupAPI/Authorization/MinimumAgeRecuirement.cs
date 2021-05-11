using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Authorization
{
    public class MinimumAgeRecuirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRecuirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }
}
