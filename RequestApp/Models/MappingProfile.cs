using AutoMapper;
using RequestApp.Domain;
using System;

namespace RequestApp.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Request, RequestResponse>()
                .ReverseMap();
        }
    }
}
