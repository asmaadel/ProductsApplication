using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication
{
    public class OrderProductProfile : Profile
    {
        public OrderProductProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<OrderProduct, ProductDto>()
            .ForPath(u => u.Id, opt => opt.MapFrom(x => x.ProductId))
            .ForPath(u => u.Name, opt => opt.MapFrom(x => x.Product.Name))
            .ForPath(u => u.Quantity, opt => opt.MapFrom(x => x.Quantity))
            .ForPath(u => u.valid, opt => opt.MapFrom(x => x.Product.valid))
            .ForPath(u => u.CreatedDate, opt => opt.MapFrom(x => x.Product.CreatedDate))
            .ForPath(u => u.Description, opt => opt.MapFrom(x => x.Product.Description))
            .ForPath(u => u.Price, opt => opt.MapFrom(x => x.Product.Price))
            ;


        }
    }
}
