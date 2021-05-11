using AutoMapper;
using MeetupAPI.Entities;
using MeetupAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI
{
    public class MeetupProfile : Profile
    {
        public MeetupProfile()
        {
            CreateMap<Meetup, MeetupDetailDto>()
                .ForMember(m => m.City, map => map.MapFrom(f => f.Location.City))
                .ForMember(m => m.PostCode, map => map.MapFrom(f => f.Location.PostCode))
                .ForMember(m => m.Street, map => map.MapFrom(f => f.Location.Street));

            CreateMap<MeetupDto, Meetup>();

            CreateMap<LectureDto, Lecture>().ReverseMap();
        }
    }
}
