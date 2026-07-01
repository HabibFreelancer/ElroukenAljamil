using AutoMapper;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Application.Mappings
{
    public class ListingProfile : Profile
    {
        public ListingProfile()
        {
            CreateMap<Listing, ListingDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.City))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.Longitude))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency));

            CreateMap<Listing, ListingSummaryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.City))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                    src.ImageUrls.Any() ? src.ImageUrls.First() : null));
        }
    }
}
