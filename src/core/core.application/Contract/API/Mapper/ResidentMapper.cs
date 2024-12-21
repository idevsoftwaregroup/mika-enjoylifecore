using common.defination.Utility;
using core.application.Contract.API.DTO.Party.Resident;
using core.domain.entity.partyModels;

namespace core.application.Contract.API.Mapper;

public static class ResidentMapper
{

    public static ResidentGetResponse ConvertResidentModelTOResidentGetResponse(this ResidentModel value)
    {
        return new()
        {
            FromDate = value.FromDate.ConvertGeoToJalaiSimple(),
            Id = value.Id,
            IsHead = value.IsHead,
            Renting = value.Renting,
            ToDate = value.ToDate?.ConvertGeoToJalaiSimple(),
            UnitId = value.Unit.Id,
            UserId = value.User != null ? value.User.Id:0,
            User= value.User!=null? value.User.MapUserModelToUserGetResponse():null
        };
    }

    public static ResidentModel ConvertResidentCreateRequestToModel(this ResidentCreateRequest value)
    {
        return new()
        {
            User = new() { Id = value.UserId },
            Unit = new() { Id = value.UnitId },
            FromDate = value.FromDate,
            IsHead = value.IsHead,
            Renting = value.Renting,
            ToDate = value.ToDate
        };
    }

    public static ResidentModel ConvertResidentUpdateRequestToModel(this ResidentUpdateRequest value)
    {
        return new()
        {
            Id = value.Id,
            User = new() { Id = value.UserId },
            Unit = new() { Id = value.UnitId },
            FromDate = value.FromDate,
            IsHead = value.IsHead,
            Renting = value.Renting,
            ToDate = value.ToDate
        };
    }
}
