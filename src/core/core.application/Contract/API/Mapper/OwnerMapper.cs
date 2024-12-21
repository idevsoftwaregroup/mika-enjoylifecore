using common.defination.Utility;
using core.application.Contract.API.DTO.Party.Owner;
using core.domain.entity.partyModels;

namespace core.application.Contract.API.Mapper;

public static class OwnerMapper
{

    public static OwnerGetResponse ConvertOwnerModelToGetResponse(this OwnerModel value)
    {
        return new OwnerGetResponse
        {
            FromDate = value.FromDate.ConvertGeoToJalaiSimple(),
            Id = value.Id,
            Percentage = value.Percentage,
            ToDate = value.ToDate.ConvertGeoToJalaiSimple(),
            UnitID = value.Unit.Id,
            UserID = value.User.Id,
            User = value.User != null ? value.User.MapUserModelToUserGetResponse() : null
        };
    }

    public static OwnerModel ConvertCreateRequestToModel(this OwnerCreateRequest value)
    {
        return new OwnerModel
        {
            FromDate = value.FromDate,
            Percentage = value.Percentage,
            ToDate = value.ToDate,
            Unit = new() { Id = value.UnitId },
            User = new() { Id = value.UserId }
        };
    }

    public static OwnerModel ConvertOwnerModelToGetResponse(this OwnerUpdateRequest value)
    {
        return new OwnerModel
        {
            FromDate = value.FromDate,
            Id = value.Id,
            Percentage = value.Percentage,
            ToDate = value.ToDate,
            Unit = new() { Id = value.UnitId },
            User = new() { Id = value.UserId }
        };
    }
}
