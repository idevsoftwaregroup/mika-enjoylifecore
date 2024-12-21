using common.defination.Utility;
using core.application.Contract.API.DTO.EnjoyEvent;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.enums;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace core.application.Contract.API.Mapper;

public static class EnjoyEventMapper
{
    public static GetEnjoyEventDetailResponseDTO ConvertEnjoyEventToGetEnjoyEventDetailDTO(this EventModel value, int unitId , List<SessionDTO>? sessionsDTOs)
    {
        var result = new GetEnjoyEventDetailResponseDTO()
        {
            Id = value.Id,
            Title = value.Name,           
            Date = value.StartDate.ToString("d yyyy ,MMMM",new CultureInfo("fa-IR")),
            Price = "",
            Gender = value.GenderType switch
            {
                GenderType.FAMALE => "مخصوص بانوان",
                GenderType.MALE => "مخصوص آقایان",
                GenderType.ALL => "عمومی",
                _ => "نامشخص"
            },
            Place = value.Place,
            Period = value.Period,
            WebsiteUrl = value.WebsiteUrl,
            SessionsDesc = value.SessionDescription,
            UnitId = unitId,
            Sessions = sessionsDTOs,
            
        };
        var now = DateTime.Now;
        if(now > value.ReservationStartDate)
        {
            if(now > value.EndDate)
            {
                var content = value.EventContent.Where(c => c.BodyType == BodyType.ARCHIVED).FirstOrDefault();
                result.Description = content?.ContentBody;
                result.Status = EventState.ARCHIVED;
                result.BannerData = content is null ? new List<BannerDTO>() : content.Media.Select(m=> new BannerDTO() { Alt = m.Alt, Src=m.Url, Type= m.Type}).ToList();
            }
            else
            {
                var content = value.EventContent.Where(c => c.BodyType == BodyType.ONGOING).FirstOrDefault();
                result.Description = content?.ContentBody;
                result.Status = EventState.ONGOING;
                result.BannerData = content is null ? new List<BannerDTO>() : content.Media.Select(m => new BannerDTO() { Alt = m.Alt, Src = m.Url, Type = m.Type }).ToList();

            }
        }
        else
        {
            var content = value.EventContent.Where(c => c.BodyType == BodyType.COMMINGSOON).FirstOrDefault();
            result.Description = content?.ContentBody;
            result.Status = EventState.COMMINGSOON;
            result.BannerData = content is null ? new List<BannerDTO>() : content.Media.Select(m => new BannerDTO() { Alt = m.Alt, Src = m.Url, Type = m.Type }).ToList();

        }
 
        
        return result;

    }

    public static EventTabDto ConvertEventModelToEventTabDTO(this EventModel value, int unitId)
    {
        var currentTime = DateTime.Now;
        BodyType thumbnailType = BodyType.THUMBNAIL;
        int _Status = -1;
        _Status = (value.PublishDate <= currentTime && value.ReservationStartDate > currentTime) ? 0 :
        (value.PublishDate <= currentTime && value.ReservationStartDate <= currentTime &&
        value.EndDate > currentTime && (value.EventSession != null &&
        !value.EventSession.Any(t => t.Tickets != null && t.Tickets.Any(t => t.Unit?.Id == unitId)))) ? 1 :
        (value.PublishDate <= currentTime && value.EndDate < currentTime) ? 2 :
        (value.PublishDate <= currentTime && value.ReservationStartDate <= currentTime &&
        value.EndDate > currentTime && (value.EventSession != null && value.EventSession.Any(t => t.Tickets != null && t.Tickets.Any(t => t.Unit?.Id == unitId)))) ? 3 : -1;

        EventTabDto result = new EventTabDto()
        {
            Id = value.Id,
            Title = value.Name,
            Date = _Status==0? null:value.StartDate.ConvertGeoToJalaiSimple(),
            Poster = value.EventContent
                            .Where(c => c.BodyType == thumbnailType)?
                            .FirstOrDefault()?.Media?
                            .FirstOrDefault()?.Url,
            Status = _Status
        };
        return result;
    }


    public static SessionDTO ConvertSessionToSessionDTO(this EventSessionModel value,
                                                        int totalNumOfReserved,
                                                        int numOfFemale,int numOfMale,
                                                        int unitsAllowedNum, int unitUsedTickets,        
                                                        int numOfGuestMale, int numOfGuestFemale, int unitGuestUsedTickets, int unitsGuestAllowedNum)
    {

        IFormatProvider formatProvider = new CultureInfo("fa-IR");

        var result = new SessionDTO() {
            Id = value.Id,
            Gender = value.GenderType switch
            {
                GenderType.FAMALE => "مخصوص بانوان",
                GenderType.MALE => "مخصوص آقایان",
                GenderType.ALL => "عمومی",
                _ => "نامشخص"
            },
            Date = value.StartTime.ToString("MMMM d , dddd", formatProvider),
            StartTime = value.StartTime.ToString("HH:mm", formatProvider),
            EndTime = value.EndTime.ToString("HH:mm",formatProvider),
            IsReserved = numOfFemale+numOfMale > 0,
            Expired = DateTime.Now > value.EndTime,
            Capacity = $"{totalNumOfReserved}/{value.Capacity}",
            RemainingCapacity = value.Capacity - totalNumOfReserved,
            TotalCapacity= value.Capacity,
            Ticket = unitsAllowedNum,
            GuestTicket = unitsGuestAllowedNum,
            Location = value.Place,
            MenCount = numOfMale,
            WomenCount = numOfFemale,
            UnitUsedTicket = unitUsedTickets,  
            GuestMenCount = numOfGuestMale,
            GuestWomenCount = numOfGuestFemale,
            UnitGuestUsedTicket= unitGuestUsedTickets
        };

        result.Time = $"{result.StartTime}-{result.EndTime}";

        return result;
    }

    public static EventTabDto ConvertEventModelToEventSliderDTO(this EventModel value)
    {
        var currentTime = DateTime.Now;
        BodyType thumbnailType = BodyType.THUMBNAIL;
        int _Status = -1;
        _Status = (value.PublishDate <= currentTime && value.ReservationStartDate > currentTime) ? 0 :
        (value.PublishDate <= currentTime && value.ReservationStartDate <= currentTime &&
        value.EndDate > currentTime) ? 1 :
        (value.PublishDate <= currentTime && value.EndDate < currentTime) ? 2  : -1;

        EventTabDto result = new EventTabDto()
        {
            Id = value.Id,
            Title = value.Name,
            Date = _Status == 0 ? null : value.StartDate.ConvertGeoToJalaiSimple(),
            Poster = value.EventContent
                            .Where(c => c.BodyType == thumbnailType)?
                            .FirstOrDefault()?.Media?
                            .FirstOrDefault()?.Url,
            Status = _Status
        };

        return result;
    }
}
