using MeetupAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Identity
{
    public interface IJwtProvider
    {
        string GenerateJwtToken(User user);
    }
}
