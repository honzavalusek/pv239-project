using Geolocation;
using MalyFarmar.Api.DAL.Models;
using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;

namespace MalyFarmar.Api.BusinessLayer.Mappers;

public static class ProductMapper
{
    public static ProductDetailViewDto MapToDetailViewDto(this Product entity)
    {
        return new ProductDetailViewDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TotalAmount = entity.TotalAmount,
            RemainingAmount = entity.RemainingAmount,
            Unit = entity.Unit,
            PricePerUnit = entity.PricePerUnit,
            Seller = entity.Seller.MapToViewDto(),
            ImageUrl = entity.ImageUrl
        };
    }

    public static ProductListViewDto MapToListViewDto(this Product entity, Coordinate? userLocation = null)
    {
        double? distance = null;

        if (userLocation != null)
        {
            var sellerLocation = new Coordinate();

            if (entity.Seller is { LocationLatitude: not null, LocationLongitude: not null })
            {
                sellerLocation = new Coordinate(
                    (double)entity.Seller.LocationLatitude,
                    (double)entity.Seller.LocationLongitude
                );
            }

            distance = GeoCalculator.GetDistance(
                sellerLocation,
                (Coordinate)userLocation,
                0,
                DistanceUnit.Meters
            );
        }

        return new ProductListViewDto
        {
            Id = entity.Id,
            Name = entity.Name,
            RemainingAmount = entity.RemainingAmount,
            Unit = entity.Unit,
            PricePerUnit = entity.PricePerUnit,
            DistanceInMeters = distance,
            ImageUrl = entity.ImageUrl
        };
    }

    public static Product MapToEntity(this ProductCreateDto dto, string? imageUrl = null)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            TotalAmount = dto.TotalAmount,
            RemainingAmount = dto.TotalAmount,
            Unit = dto.Unit,
            PricePerUnit = dto.PricePerUnit,
            SellerId = dto.SellerId,
            ImageUrl = imageUrl
        };
    }
}