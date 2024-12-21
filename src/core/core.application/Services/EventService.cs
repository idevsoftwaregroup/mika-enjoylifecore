using core.application.Contract.API.DTO.EnjoyEvent;
using core.application.Contract.API.DTO.Ticket;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.infrastructure;
using core.application.Contract.Infrastructure;
using core.application.Framework;
using core.domain.entity;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.enums;
using core.domain.entity.partyModels;
using core.domain.entity.structureModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Runtime.CompilerServices;

namespace core.application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IRepository _repository;
    public EventService(IEventRepository eventRepository, IUnitRepository unitRepository, IRepository repository)
    {
        _eventRepository = eventRepository;
        _unitRepository = unitRepository;
        _repository = repository;
    }
    public async Task<int> CreateEvent(CreateEnjoyEventRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        EventModel EventModel = new EventModel()
        {
            ComplexId = requestDTO.ComplexId,
            Name = requestDTO.Name,
            ReservationStartDate = requestDTO.ReservationStartDate,
            PublishDate = requestDTO.PublishDate,
            StartDate = requestDTO.StartDate,
            EndDate = requestDTO.EndDate,
            Period = requestDTO.Period,
            LockTimeout = requestDTO.LockTimeout,
            GenderType = requestDTO.GenderType,
            WebsiteUrl = requestDTO.WebsiteUrl,
            SupportPhoneNumber = requestDTO.SupportPhoneNumber,
            MandatoryConsecutiveReservation = requestDTO.MandatoryConsecutiveReservation,
            OwnersMaxReservations = requestDTO.OwnersMaxReservations,
            repeatSessionReservation = requestDTO.repeatSessionReservation,
            Place = requestDTO.Place,
            IsPinned = requestDTO.IsPinned,
            SessionDescription = requestDTO.SessionDescription
        };

        return await _eventRepository.AddEnjoyEventAsync(EventModel, cancellationToken);
    }
    public async Task<int> AddEventContent(CreateEventContentRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        EventModel eventModel = await _eventRepository.GetEventAsync(requestDTO.EventId);
        if (eventModel.EventContent == null)
        {
            eventModel.EventContent = new List<EventContentModel>();
        }
        EventContentModel eventContentModel = new EventContentModel
        {
            ContentBody = requestDTO.BodyType != BodyType.THUMBNAIL ? requestDTO.ContentBody : null,
            BodyType = requestDTO.BodyType,
            Media = requestDTO.Media?.Select(media => new EventMediaModel
            {
                Url = media.Url,
                Alt = media.Alt,
                Type = media.Type
            }).ToList() ?? new List<EventMediaModel>()
        };
        eventModel.EventContent.Add(eventContentModel);
        return await _eventRepository.UpdateEventAsync(eventModel);

    }
    public async Task<int> AddEventSession(CreateSessionRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        EventModel Event = await _eventRepository.GetEventAsync(requestDTO.EventId);

        if (Event == null) throw new Exception("event not found");
        if (Event.EventSession == null)
        {
            Event.EventSession = new List<EventSessionModel>();
        }


        EventSessionModel session = new EventSessionModel()
        {
            StartTime = requestDTO.StartTime,
            EndTime = requestDTO.EndTime,
            Tickets = new List<EventTicketModel>(),
            Capacity = requestDTO.Capacity,
            Place = requestDTO.Place,
            GenderType = requestDTO.GenderType,
        };

        Event.EventSession.Add(session);
        return await _eventRepository.UpdateEventAsync(Event);
    }
    public async Task<int> SetEventParty(SetEventPartyDTO requestDTO)
    {
        EventModel Event = await _eventRepository.GetEventAsync(requestDTO.EventId);
        UnitModel Unit = await _unitRepository.GetByIdAsync(requestDTO.UnitId);
        if (Unit == null || Event == null) throw new Exception("not found");

        EventPartyModel party = await _eventRepository.GetEventPartyAsync(requestDTO.EventId, requestDTO.UnitId);
        if (party == null)
        {
            EventPartyModel EventParty = new EventPartyModel()
            {
                Event = Event,
                unit = Unit,
                GuestCount = requestDTO.MaxGuestCount
            };
            return await _eventRepository.AddEventPartyAsync(EventParty);
        }
        else
        {
            party.GuestCount = requestDTO.MaxGuestCount;
            return await _eventRepository.UpdateEventPartyAsync(party);
        }
    }
    public async Task<OperationResult<TicketRequestDTO?>> SaveEventTicket(TicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        //new - edit - delete
        OperationResult<TicketRequestDTO?> Op = new("SaveEventTicket");
        try
        {
            int userId = requestDTO.UserId ?? 0;
            EventSessionModel session = await _eventRepository.GetEventSessionAsync(requestDTO.SessionId);
            if (session == null)
            {
                return Op.Failed("شناسه ایونت بدرستی وارد نشده است", System.Net.HttpStatusCode.NotFound);
            }
            EventModel Event = await _eventRepository.GetEventAsync(session.Event.Id);
            if (Event == null)
            {
                return Op.Failed("شناسه ایونت بدرستی وارد نشده است", System.Net.HttpStatusCode.NotFound);
            }
            ResidentModel head = await _unitRepository.IsUnitHeadAsync(requestDTO.UnitId, userId);
            if (head == null)
            {
                return Op.Failed("دسترسی برای ثبت ، بروزرسانی یا حذف رزرو ایونت برای کاربری شما وجود ندارد", System.Net.HttpStatusCode.Forbidden);
            }
            if (!session.IsActive)
            {
                return Op.Failed("ایونت غیر فعال است", HttpStatusCode.Conflict);
            }
            var remainedCapacity = 0;
            try
            {
                remainedCapacity = session.Capacity - (session.Tickets?.Sum(t => t.MaleNum + t.FemaleNum + t.GuestMaleNum + t.GuestFemaleNum) ?? 0);
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت رزرو ایونت با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
            int unitMaxTicketCount = await GetNumberOfAllowedReservationsForUnit(userId, requestDTO.UnitId, 0, true);//must calculate        
            var eventParty = await _eventRepository.GetEventPartyAsync(session.Event.Id, requestDTO.UnitId);
            int unitGuestMaxTicketCount = eventParty?.GuestCount ?? 0;
            if (session.StartTime - session.Event.LockTimeout < DateTime.Now)
            {
                return Op.Failed("انقضای زمان رزور به پایان رسیده است", HttpStatusCode.Forbidden);
            }
            #region unitUsedTicketCount
            int unitUsedTicketCount = 0;
            if (Event.repeatSessionReservation == true)
            {
                unitUsedTicketCount = (session.Tickets?.Where(t => t.Unit?.Id == requestDTO.UnitId)?.Sum(t => t.FemaleNum + t.MaleNum) ?? 0);
            }
            else
            {
                unitUsedTicketCount = Event?.EventSession?.Sum(s =>
                                s.Tickets?.Where(t => t.Unit?.Id == requestDTO.UnitId)?.Sum(t => t.FemaleNum + t.MaleNum) ?? 0) ?? 0;
            }
            #endregion
            #region unitGuestUsedTicketCount
            int unitGuestUsedTicketCount = 0;
            if (unitGuestMaxTicketCount > 0)
            {
                if (Event.repeatSessionReservation == true)
                {
                    unitGuestUsedTicketCount = (session.Tickets?.Where(t => t.Unit?.Id == requestDTO.UnitId)?.Sum(t => t.GuestMaleNum + t.GuestFemaleNum) ?? 0);
                }
                else
                {
                    unitGuestUsedTicketCount = Event?.EventSession?.Sum(s =>
                                    s.Tickets?.Where(t => t.Unit?.Id == requestDTO.UnitId)?.Sum(t => t.GuestMaleNum + t.GuestFemaleNum) ?? 0) ?? 0;
                }
            }
            #endregion
            if (remainedCapacity < requestDTO.MaleNum + requestDTO.FemaleNum + unitUsedTicketCount +
                        requestDTO.GuestMaleNum + requestDTO.GuestMaleNum + unitGuestUsedTicketCount)
            {
                return Op.Failed("تعداد رزرو درخواست شده از ظرفیت ایونت بیشتر است",HttpStatusCode.Conflict);
            }
            if (unitMaxTicketCount < requestDTO.MaleNum + requestDTO.FemaleNum + unitUsedTicketCount)
            {
                return Op.Failed("تعداد رزرو درخواست شده از ظرفیت ایونت بیشتر است", HttpStatusCode.Conflict);
            }
            if (unitGuestMaxTicketCount < requestDTO.GuestMaleNum + requestDTO.GuestFemaleNum + unitGuestUsedTicketCount)
            {
                return Op.Failed("تعداد رزرو درخواست شده از ظرفیت ایونت بیشتر است", HttpStatusCode.Conflict);
            }
            if (unitGuestMaxTicketCount > 0 && requestDTO.MaleNum + requestDTO.FemaleNum == 0)
            {
                return Op.Failed("رزرو ایونت برای ساکنین نمیتواند صفر باشد", HttpStatusCode.Conflict);
            }
            if (Event.GenderType == GenderType.FAMALE)
            {
                if (requestDTO.MaleNum > 0)
                {
                    return Op.Failed("رزرو تنها برای بانوان مجاز است", HttpStatusCode.Conflict);
                }
                if (requestDTO.GuestMaleNum > 0)
                {
                    return Op.Failed("رزرو تنها برای بانوان مجاز است", HttpStatusCode.Conflict);
                }
            }
            if (Event.GenderType == GenderType.MALE)
            {
                if (requestDTO.FemaleNum > 0)
                {
                    return Op.Failed("رزرو تنها برای آقایان مجاز است", HttpStatusCode.Conflict);
                }
                if (requestDTO.GuestFemaleNum > 0)
                {
                    return Op.Failed("رزرو تنها برای آقایان مجاز است", HttpStatusCode.Conflict);
                }
            }
            EventTicketModel existingTicket = await _eventRepository.GetEventTicketAsync(requestDTO.SessionId, requestDTO.UnitId);
            if (existingTicket != null)
            {
                existingTicket.MaleNum = requestDTO.MaleNum;
                existingTicket.FemaleNum = requestDTO.FemaleNum;
                existingTicket.LastModified = DateTime.Now;
                existingTicket.GuestMaleNum = requestDTO.GuestMaleNum;
                existingTicket.GuestFemaleNum = requestDTO.GuestFemaleNum;
                if (existingTicket.MaleNum + existingTicket.FemaleNum == 0)
                {
                    //delete
                    await _eventRepository.DeleteEventTicketAsync(existingTicket.Id);
                    return Op.Succeed("حذف رزرو ایونت با موفقیت انجام شد");
                }
                else
                {
                    //edit
                    await _eventRepository.UpdateEventTicketAsync(existingTicket);
                    return Op.Succeed("بروزرسانی رزرو ایونت با موفقیت انجام شد",requestDTO);
                }
            }
            else
            {
                EventTicketModel newEventTicket = new ()
                {
                    Unit = head.Unit,
                    User = head.User,
                    MaleNum = requestDTO.MaleNum,
                    FemaleNum = requestDTO.FemaleNum,
                    GuestFemaleNum = requestDTO.GuestFemaleNum,
                    GuestMaleNum = requestDTO.GuestMaleNum,
                    Session = session,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                };

                long tId = await _eventRepository.AddEventTicketAsync(newEventTicket);
                requestDTO.Id = tId;
                return Op.Failed("ثبت رزرو ایونت با موفقیت انجام شد", requestDTO);
            }
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت رزرو ایونت با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }
    public async Task<List<EventTabDto>?> GetEvents(int unitId, int tabId, CancellationToken cancellationToken = default)
    {
        if (tabId == 1)
        {
            var CommingSoon = await _eventRepository.GetEventAsync(unitId, 0);
            var OnGoing = await _eventRepository.GetEventAsync(unitId, 1);
            var Tab1 = CommingSoon.Union(OnGoing).Select(x => x.ConvertEventModelToEventTabDTO(unitId)).ToList();
            return Tab1;
        }
        if (tabId == 2)
        {
            var MyEvent = await _eventRepository.GetEventAsync(unitId, 3);
            return MyEvent.Select(x => x.ConvertEventModelToEventTabDTO(unitId)).ToList(); ;
        }
        if (tabId == 3)
        {
            var Archived = await _eventRepository.GetEventAsync(unitId, 2);
            return Archived.Select(x => x.ConvertEventModelToEventTabDTO(unitId)).ToList();
        }
        return null;
    }
    public async Task<EventModel> UpdateEventAsync(int id, UpdateEnjoyEventDTO updateEnjoyEventDTO, CancellationToken cancellationToken = default)
    {
        EventModel? eventModel = await _eventRepository.GetEventAsync(id);
        if (eventModel == null) throw new Exception("event not found");
        eventModel.ComplexId = updateEnjoyEventDTO.ComplexId;
        eventModel.Name = updateEnjoyEventDTO.Name;
        eventModel.ReservationStartDate = updateEnjoyEventDTO.ReservationStartDate;
        eventModel.PublishDate = updateEnjoyEventDTO.PublishDate;
        eventModel.StartDate = updateEnjoyEventDTO.StartDate;
        eventModel.EndDate = updateEnjoyEventDTO.EndDate;
        eventModel.Period = updateEnjoyEventDTO.Period;
        eventModel.LockTimeout = updateEnjoyEventDTO.LockTimeout;
        eventModel.GenderType = updateEnjoyEventDTO.GenderType;
        eventModel.WebsiteUrl = updateEnjoyEventDTO.WebsiteUrl;
        eventModel.SupportPhoneNumber = updateEnjoyEventDTO.SupportPhoneNumber;
        eventModel.OwnersMaxReservations = updateEnjoyEventDTO.OwnersMaxReservations;
        eventModel.MandatoryConsecutiveReservation = updateEnjoyEventDTO.MandatoryConsecutiveReservation;
        eventModel.repeatSessionReservation = updateEnjoyEventDTO.repeatSessionReservation;
        eventModel.Place = updateEnjoyEventDTO.Place;
        eventModel.SessionDescription = updateEnjoyEventDTO.SessionDescription;
        eventModel.IsPinned = updateEnjoyEventDTO.IsPinned;

        int a = await _eventRepository.UpdateEventAsync(eventModel);

        if (a < 0) throw new Exception("update faild");

        return eventModel;

    }
    public async Task<EventContentModel> UpdateEventContent(int eventId, int id, UpdateEventContentDTO updateEventContentDTO, CancellationToken cancellationToken = default)
    {
        EventModel eventModel = await _eventRepository.GetEventAsync(eventId) ?? throw new Exception("event not found");

        EventContentModel eventContent = eventModel.EventContent.Where(x => x.Id == id).FirstOrDefault() ?? throw new Exception("event content not found");

        eventContent.ContentBody = updateEventContentDTO.ContentBody;
        eventContent.BodyType = updateEventContentDTO.BodyType;

        await _eventRepository.UpdateEventAsync(eventModel);

        //return eventModel.ConvertEventModelToEventTabDTO(unitId);
        return eventContent;
    }
    public async Task<EventSessionModel> UpdateEventSession(long id, UpdateSessionDTO updateSessionDTO, CancellationToken cancellationToken = default)
    {
        EventSessionModel eventSessionModel = await _eventRepository.GetEventSessionAsync(id) ?? throw new Exception("session not found");
        eventSessionModel.Capacity = updateSessionDTO.Capacity;
        eventSessionModel.Place = updateSessionDTO.Place;
        eventSessionModel.EndTime = updateSessionDTO.EndTime;
        eventSessionModel.StartTime = updateSessionDTO.StartTime;
        eventSessionModel.GenderType = updateSessionDTO.GenderType;

        await _eventRepository.UpdateSessionAsync(eventSessionModel);

        return eventSessionModel;
    }
    public async Task<EventTicketModel> UpdateEventTicket(long id, UpdateEventTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        EventTicketModel eventTicketModel = await _eventRepository.GetEventTicketAsync(id) ?? throw new Exception("ticket not found");
        int allowedPersonNum = _repository.Residents.Where(x => x.Unit.Id == eventTicketModel.Unit.Id).Count(); // should check if its owner or not
        //allowedPersonNum = allowedPersonNum + _repository.Owners.Where(x=>x.Unit.Id == eventTicketModel.Id).Count();


        CheckCapacity(eventTicketModel.Session.GenderType, allowedPersonNum, requestDTO.femaleNum, requestDTO.maleNum);

        int remainingCapacity = _repository.EventSessions.Where(s => s.Id == eventTicketModel.Session.Id).Sum(s => s.Tickets.Sum(t => t.FemaleNum + t.MaleNum));
        remainingCapacity = remainingCapacity + eventTicketModel.MaleNum + eventTicketModel.FemaleNum;
        if (requestDTO.femaleNum + requestDTO.maleNum > remainingCapacity) throw new Exception("exceeding session capacity");

        eventTicketModel.FemaleNum = requestDTO.femaleNum;
        eventTicketModel.MaleNum = requestDTO.maleNum;

        await _eventRepository.UpdateEventTicketAsync(eventTicketModel);

        return eventTicketModel;


    }
    private async Task checkTicketValidity()
    {

    }

    private async Task<int> GetNumberOfAllowedReservationsForUnit(int userId, int unitId, int ownersMax, bool viewOnly)
    {

        var residents = _repository.Residents.Where(r => r.User.Id == userId && r.Unit.Id == unitId).ToList();


        if (residents.Count > 0)
        {
            var headresidency = residents.Where(r => r.IsHead).FirstOrDefault();

            if (viewOnly || headresidency is not null)
            {
                return _repository.Residents.Where(r => r.Unit.Id == unitId).Count();
            }
            else throw new Exception("u r not head");

        }
        else
        {
            if (_repository.Residents.Any(r => r.User.Id == userId)) throw new Exception("u r a resident in atleast another unit");

            var ownerships = _repository.Owners.Where(o => o.Unit.Id == unitId && o.User.Id == userId).ToList();

            if (ownerships.Count > 0)
            {
                if (_repository.Residents.Any(r => r.Unit.Id == unitId))
                {
                    return ownersMax;
                }
                else return EventModel.EmtpyUnitOwnerMaxReservation;
            }
            else throw new Exception("u dont have any units");
        }

    }

    public async Task<EventMediaModel> UpdateEventMedia(int id, UpdateEventMediaDTO updateEventMediaDTO, CancellationToken cancellationToken = default)
    {
        EventMediaModel mediaModel = await _eventRepository.GetEventMediaAsync(id) ?? throw new Exception();

        mediaModel.Url = updateEventMediaDTO.Url;
        mediaModel.Alt = updateEventMediaDTO.Alt;
        mediaModel.Type = updateEventMediaDTO.Type;

        await _eventRepository.UpdateMediaAsync(mediaModel);

        return mediaModel;
    }
    private void CheckCapacity(GenderType genderType, int cap, int femaleNum, int maleNum)
    {
        int sum = femaleNum + maleNum;
        if (genderType == GenderType.MALE)
        {
            if (femaleNum != 0) throw new Exception();
            else if (maleNum > cap) throw new Exception();
        }
        else if (genderType == GenderType.FAMALE)
        {
            if (maleNum != 0) throw new Exception();
            else if (femaleNum > cap) throw new Exception();
        }
        else
        {
            if (maleNum + femaleNum > cap) throw new Exception();
        }
    }
    public async Task DeleteEvent(int id)
    {
        var eventModel = await _eventRepository.GetEventAsync(id) ?? throw new Exception("event not found");
        await _eventRepository.DeleteEventAsync(eventModel);

    }
    //public async Task<GetReservationResponseDTO> CreateReservation(CreateReservationRequestDTO requestDTO, CancellationToken cancellationToken = default)
    //{

    //}
    public async Task DeleteEventSession(int id)
    {
        var eventSession = await _eventRepository.GetEventSessionAsync(id) ?? throw new Exception("event session not found");
        await _eventRepository.DeleteEventSessionAsync(eventSession);
    }
    public async Task DeleteEventTicket(long id)
    {
        await _eventRepository.DeleteEventTicketAsync(id);

    }
    public async Task DeleteEventMedia(int id)
    {
        var eventMedia = await _eventRepository.GetEventMediaAsync(id) ?? throw new Exception("media not found");
        await _eventRepository.DeleteEventMediaAsync(eventMedia);
    }

    public async Task<List<EventTabDto>?> GetSliderEvents(int ComplexId, CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetSliderEventsAsync(ComplexId);
        return events.Select(x => x.ConvertEventModelToEventSliderDTO()).ToList();
    }
    public async Task<GetEnjoyEventDetailResponseDTO> GetEnjoyEventDetail(int eventId, int unitId, int userId, CancellationToken cancellationToken = default)
    {
        if (unitId == 0)
        {
            var eventModel = await _repository.Events.Where(e => e.Id == eventId)
                       .Include(eventModel => eventModel.EventContent).ThenInclude(c => c.Media)
                       .SingleOrDefaultAsync(cancellationToken)
                       ?? throw new Exception("event not found");
            return eventModel.ConvertEnjoyEventToGetEnjoyEventDetailDTO(unitId, null);
        }
        else
        {
            var eventModel = await _repository.Events.Where(e => e.Id == eventId)
                        .Include(e => e.EventSession).ThenInclude(s => s.Tickets).ThenInclude(t => t.Unit) // do a better way to not load all extra units
                        .Include(eventModel => eventModel.EventContent).ThenInclude(c => c.Media)
                        .SingleOrDefaultAsync(cancellationToken)
                        ?? throw new Exception("event not found");

            //var unitTickets = _repository.EventSessions.Where(s => s.Event.Id == eventId).SelectMany(s => s.Tickets).Where(t => t.Unit.Id == unitId).ToList();



            List<SessionDTO> sessionDTOs = new();

            int unitallowed = await GetNumberOfAllowedReservationsForUnit(userId, unitId, eventModel.OwnersMaxReservations, true);

            var eventParty = await _eventRepository.GetEventPartyAsync(eventId, unitId);
            int unitGuestallowed = eventParty?.GuestCount ?? 0;


            if (!eventModel.repeatSessionReservation)
            {

                int unitUsedTickets = eventModel.EventSession.Sum(s => s.Tickets.Where(t => t.Unit.Id == unitId).Sum(t => t.FemaleNum + t.MaleNum)); //unitTickets.Sum(t => t.FemaleNum);
                int unitGuestUsedTickets = eventModel.EventSession.Sum(s => s.Tickets.Where(t => t.Unit.Id == unitId).Sum(t => t.GuestMaleNum + t.GuestMaleNum)); //unitTickets.Sum(t => t.FemaleNum);
                sessionDTOs = eventModel.EventSession.Select(s => s.ConvertSessionToSessionDTO(s.Tickets.Sum(t => t.MaleNum + t.FemaleNum + t.GuestFemaleNum + t.GuestMaleNum),
                                                                                               s.Tickets.Where(t => t.Unit.Id == unitId).Sum(t => t.FemaleNum),
                                                                                               s.Tickets.Where(t => t.Unit.Id == unitId).Sum(t => t.MaleNum),
                                                                                               unitallowed,
                                                                                               unitUsedTickets,
                                                                                               s.Tickets.Where(t => t.Unit.Id == unitId).Sum(t => t.GuestMaleNum),
                                                                                               s.Tickets.Where(t => t.Unit.Id == unitId).Sum(t => t.GuestFemaleNum),
                                                                                               unitGuestUsedTickets,
                                                                                               unitGuestallowed)).ToList();

            }
            else
            {
                foreach (EventSessionModel sessionModel in eventModel.EventSession)
                {
                    var malefemale = sessionModel.Tickets.Where(t => t.Unit.Id == unitId).Select(t => new { t.FemaleNum, t.MaleNum, t.GuestFemaleNum, t.GuestMaleNum }).SingleOrDefault() ?? new { FemaleNum = 0, MaleNum = 0, GuestFemaleNum = 0, GuestMaleNum = 0 };
                    int totalReserved = sessionModel.Tickets.Sum(t => t.FemaleNum + t.MaleNum + t.GuestMaleNum + t.GuestFemaleNum);
                    sessionDTOs.Add(sessionModel.ConvertSessionToSessionDTO(totalReserved,
                                                                            malefemale.FemaleNum,
                                                                            malefemale.MaleNum,
                                                                            unitallowed,
                                                                            malefemale.FemaleNum + malefemale.MaleNum,
                                                                            malefemale.GuestMaleNum,
                                                                            malefemale.GuestFemaleNum,
                                                                            malefemale.GuestMaleNum + malefemale.GuestFemaleNum,
                                                                            unitGuestallowed
                                                                            ));
                }

            }


            return eventModel.ConvertEnjoyEventToGetEnjoyEventDetailDTO(unitId, sessionDTOs);
        }
    }
}
