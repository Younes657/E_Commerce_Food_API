using AutoMapper;
using E_Commerce_Food_API.Models;
using E_Commerce_Food_API.Models.DTO;

namespace E_Commerce_Food_API.Services
{
    public class AutoMapperProfile : Profile 
    {
        public AutoMapperProfile() 
        {
            CreateMap<MenuItemCreateDto, MenuItem>();
            CreateMap<MenuItem, MenuItemCreateDto>();

            CreateMap<MenuItemUpdateDto, MenuItem>();
            CreateMap<MenuItem, MenuItemUpdateDto>();

            CreateMap<OrderHeaderCreateDTO, OrderHeader>();
            CreateMap<OrderHeader, OrderHeaderCreateDTO>();

            CreateMap<OrderHeaderUpdateDTO, OrderHeader>();
            CreateMap<OrderHeader, OrderHeaderUpdateDTO>();

            CreateMap<OrderDetailsCreateDto, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailsCreateDto>();
        }
    }
}
