using AutoMapper;
using ExchangeSimulatorBackend.Dtos;
using ExchangeSimulatorBackend.Entities;

namespace ExchangeSimulatorBackend
{
    public class ExchangeSimulatorMappingProfile : Profile
    {
        public ExchangeSimulatorMappingProfile()
        {
            CreateMap<AppUser, UserDto>();
        }

        //public RestaurantMappingProfile()
        //{
        //    //jeśli typy własciwosci pomiedzy restaurant i restaurantDto się zgadzają to automapper je zmapuje automatycznie 
        //    //source, target
        //    CreateMap<Restaurant, RestaurantDto>()
        //        .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
        //        .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
        //        .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.Street));

        //    CreateMap<Dish, DishDto>();

        //    CreateMap<CreateDishDto, Dish>();

        //    CreateMap<CreateRestaurantDto, Restaurant>()
        //        .ForMember(r => r.Address, c => c.MapFrom(dto => new Address() { City = dto.City, PostalCode = dto.PostalCode, Street = dto.Street }));



        //}
    }

}
