using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MeetupAPI.Authorization;
using MeetupAPI.Entities;
using MeetupAPI.Filters;
using MeetupAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetupAPI.Controllers
{
    [Route("api/meetup")]
    [Authorize]
    [ServiceFilter(typeof(TimeTrackFilter))]
    public class MeetupController : ControllerBase
    {
        private readonly MeetupContext _meetupContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public MeetupController(MeetupContext meetupContext, IMapper mapper, IAuthorizationService authorizationService)
        {
            _meetupContext = meetupContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [NationalityFilter("German,Russian")]
        public ActionResult<List<MeetupDetailDto>> Get()
        {
            var meetups = _meetupContext.Meetups.Include(m => m.Location).ToList();
            var meetupDtos = _mapper.Map<List<MeetupDetailDto>>(meetups);
            return Ok(meetupDtos);

            //przykłady zwracania kodu statusu
            //return StatusCode(404, meetups);
            //return NotFound(meetups);
            //HttpContext.Response.StatusCode = 404;
        }

        [HttpGet("{name}")]
        //[Authorize(Policy = "AtLeast18")]
        [NationalityFilter("Poland")]
        public ActionResult<MeetupDetailDto> Get(string name)
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Location)
                .Include(m=>m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());

            if (meetup == null)
            {
                return NotFound();
            }

            var meetupDtos = _mapper.Map<MeetupDetailDto>(meetup);
            return Ok(meetupDtos);

        }

        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        
        public ActionResult Post([FromBody]MeetupDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meetup = _mapper.Map<Meetup>(model);

            var userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            meetup.CreatedById = int.Parse(userId);

            _meetupContext.Add(meetup);
            _meetupContext.SaveChanges();

            var key = meetup.Name.Replace(" ", "-").ToLower();

            return Created("api/meetup/" + key, null);
        }

        [HttpPut("{name}")]
        public ActionResult Put(string name, [FromBody]MeetupDto model)
        {
            var meetup = _meetupContext.Meetups
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());

            if (meetup == null)
            {
                return NotFound();
            }

            var authorizationResult = _authorizationService
                .AuthorizeAsync(User, meetup, new ResourceOperationRequirement(OperationType.Update)).Result;

            if(!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            meetup.Name = model.Name;
            meetup.Organizer = model.Organizer;
            meetup.Date = model.Date;
            meetup.IsPrivate = model.IsPrivate;

            _meetupContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{name}")]
        public ActionResult Delete(string name)
        {
            var meetup = _meetupContext.Meetups
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());

            if (meetup == null)
            {
                return NotFound();
            }

            var authorizationResult = _authorizationService
                .AuthorizeAsync(User, meetup, new ResourceOperationRequirement(OperationType.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _meetupContext.Remove(meetup);
            _meetupContext.SaveChanges();

            return NoContent();
        }
    }
}