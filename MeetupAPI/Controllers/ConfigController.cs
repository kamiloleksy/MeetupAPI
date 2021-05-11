using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MeetupAPI.Controllers
{
    [Route("config")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ConfigController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpOptions("reload")]
        public ActionResult Reload()
        {
            try
            {
                ((IConfigurationRoot)configuration).Reload();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}