using MeetupAPI.Entities;
using MeetupAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {
        //controller for test

        private readonly MeetupContext _meetupContext;
        private readonly IMath _mathService;
        public TestController(MeetupContext meetupContext, IMath mathService)
        {
            _meetupContext = meetupContext;
            _mathService = mathService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            //for testing math service 
            var array = new double[] { 2, 3, 5, 8, 9 };
            var average = _mathService.Average(array);


            //test join in LINQ
            var meetups = _meetupContext.Lectures.Join(
                _meetupContext.Meetups,
                    l => l.MeetupId,
                    m => m.Id,
                    (l, m) => new { l.Author, l.Topic, m.Name }
               );


            return Ok();
        }

    }
}
