using System.Net.NetworkInformation;
using System;
using AutoMapper;
using Hotels.API.Entities;
using Hotels.API.Models.Derived;
using Hotels.API.Models;

namespace Hotels.API.Infrastructure
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<RoomEntity, Room>()
                .ForMember(dest => dest.Rate, opt =>
                   opt.MapFrom(scr => scr.Rate / 100));

            CreateMap<UserEntity, UserInfo>()
                .ForMember(desc => desc.FullName, opt =>
                    opt.MapFrom(scr => string.Concat(scr.Name, scr.SurName)));
        }
    }
}
