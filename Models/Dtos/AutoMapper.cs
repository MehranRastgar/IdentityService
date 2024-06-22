using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace IdentityService.Models.Dtos
{
  public class AutoMapper
  {
    public class UserProfile : Profile
    {
      public UserProfile()
      {
        CreateMap<ApplicationUser, UserReadDto>();
        CreateMap<ApplicationUser, UserCreateDto>();


        CreateMap<UserCreateDto, ApplicationUser>();
        CreateMap<UserUpdateDto, ApplicationUser>()
            .ForMember(dest => dest.Family, opt => opt.MapFrom(src => src.Family))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src => src.NationalNumber))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.MobileNumber)).ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
      }
    }
  }
}



// CreateMap<Animal, AnimalDto>().IncludeMembers(x => x.Address);
// CreateMap<AddressDto, AnimalDto>();

// CreateMap<Address, AddressDto>();
// CreateMap<Address, AnimalDto>();

// // From flat structure into nested
// CreateMap<CreateAnimalDto, Animal>().ForMember(d => d.Address, o => o.MapFrom(s => s));
// CreateMap<CreateAnimalDto, Address>();

// // Publish it in flat structure
// CreateMap<AnimalDto, AnimalCreated>().IncludeMembers(x => x.Address); ;
// CreateMap<AddressDto, AnimalCreated>();

// CreateMap<Animal, AnimalUpdated>();