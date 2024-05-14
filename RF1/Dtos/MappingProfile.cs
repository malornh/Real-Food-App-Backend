using AutoMapper;
using RF1.Models;
using RF1.Dtos;


namespace RF1.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Farm, FarmDto>();
            CreateMap<FarmDto, Farm>();

            CreateMap<Shop, ShopDto>();
            CreateMap<ShopDto, Shop>();

            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();

            //CreateMap<Rating, RatingDto>();
            CreateMap<RatingDto, Rating>();
        }
    }
}
