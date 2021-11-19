using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
namespace ProductsApplication
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationModel, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
            CreateMap<RegisterDto, User>()
              .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.UserName))
              .ForMember(u => u.Name, opt => opt.MapFrom(x => x.Name))
              .ForMember(u => u.Email, opt => opt.MapFrom(x => x.Email));
        } 
    }
}
