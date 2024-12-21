using core.domain.entity;
using core.domain.entity.EnjoyEventModels;

namespace core.application.Contract.API.DTO.EnjoyEvent;

public class GetEnjoyEventDetailResponseDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Date { get; set; }
    public EventState Status { get; set; }
    public string Price { get; set; }
    public string Gender { get; set; }
    public string? Place { get; set; }
    public string? Period { get; set; }
    public string WebsiteUrl { get; set; }
    public int UnitId { get; set; }
    public string SessionsDesc { get; set; }
    public List<BannerDTO> BannerData { get; set; }
    public List<SessionDTO>? Sessions { get; set; }
}

public class BannerDTO
{
    public MediaType Type { get; set; }
    public string Src { get; set; }
    public string Alt { get; set; }
}

public class SessionDTO
{
    public long Id { get; set; }
    public string Gender { get; set; }
    public string Time { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Date { get; set; }
    public bool IsReserved { get; set; }
    public bool Expired { get; set; }
    public string Capacity { get; set; }
    public int RemainingCapacity { get; set; }
    public int TotalCapacity { get; set; }
    public int Ticket { get; set; }
    public int GuestTicket { get; set; }
    public string Location { get; set; }
    public int MenCount { get; set; }
    public int WomenCount { get; set; }
    public int UnitUsedTicket { get; set; }
    public int GuestMenCount { get; set; }
    public int GuestWomenCount { get; set; }
    public int UnitGuestUsedTicket { get; set; }

}