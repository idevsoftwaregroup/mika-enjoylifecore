using common.defination.Utility;
using core.application.Contract.API.DTO.Party.Manager;
using core.domain.entity.partyModels;

namespace core.application.Contract.API.Mapper;

public static class ManagerMapper
{
    public static ManagerModel CovnertManagerUpdateRequestToModel(this ManagerUpdateRequest value)
    {
        return new()
        {
            Complex = new() { Id = value.ComplexId },
            User = new() { Id = value.UserId },
            Id = value.Id,
            FromDate = value.FromDate,
            ToDate = value.ToDate
        };
    }

    public static ManagerModel CovnertManagerCreateRequestToModel(this ManagerCreateRequest value)
    {
        return new()
        {
            Complex = new() { Id = value.ComplexId },
            User = new() { Id = value.UserId },
            FromDate = value.FromDate,
            ToDate = value.ToDate
        };
    }


    public static ManagerGetResponse ConvertManagerModelTOManagerGetResponse(this ManagerModel value)
    {
        return new()
        {
            ComplexId = value.Complex.Id,
            UserId = value.User.Id,
            Id = value.Id,
            FromDate = value.FromDate.ConvertGeoToJalaiSimple(),
            ToDate = value.ToDate?.ConvertGeoToJalaiSimple(),
            //PersianFromDate = value.FromDate.ConvertGeoToJalaiSimple(),
            //PersianToDate = value.ToDate?.ConvertGeoToJalaiSimple()
        };
    }
}
