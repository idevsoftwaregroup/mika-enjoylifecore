
using core.application.Contract.API.DTO.Reservation.Joint;
using core.application.Contract.API.DTO.Reservation.Joint.Filters;
using core.application.Contract.API.DTO.Reservation.Reservation;
using core.application.Contract.API.DTO.Reservation.Reservation.Filter;
using core.application.Contract.API.DTO.Reservation.Reservation.ServiceDTOs;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Net;

namespace core.application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    public ReservationService(IReservationRepository reservationRepository)
    {
        this._reservationRepository = reservationRepository;
    }

    public async Task<OperationResult<object>> CreateClosureJointSession(int AdminId, Request_CreateClosureJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.CreateClosureJointSession(AdminId, model, cancellationToken);
    }

    public async Task<OperationResult<object>> CreateJoint(Request_CreateJointDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.CreateJoint(model, cancellationToken);
    }

    public async Task<OperationResult<object>> CreatePrivateJointSession(int AdminId, Request_CreatePrivateJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.CreatePrivateJointSession(AdminId, model, cancellationToken);
    }

    public async Task<OperationResult<object>> CreatePublicJointSession(int AdminId, Request_CreatePublicJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.CreatePublicJointSession(AdminId, model, cancellationToken);
    }

    public async Task<OperationResult<object>> DeleteJoint(Request_DeleteJointDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.DeleteJoint(model, cancellationToken);
    }

    public async Task<OperationResult<object>> DeleteJointSession(Request_DeleteJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.DeleteJointSession(model, cancellationToken);
    }

    public async Task<OperationResult<Response_GetComplexDTO>> GetComplexes(CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetComplexes(cancellationToken);
    }

    public async Task<OperationResult<Response_GetJointDTO>> GetJoint(int JointId, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetJoint(JointId, cancellationToken);
    }

    public async Task<OperationResult<Response_GetJointDTO>> GetJoints(Filter_GetJointDTO? filter, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetJoints(filter, cancellationToken);
    }

    public async Task<OperationResult<Response_GetJointSessionByAdminDTO>> GetJointSessionsByAdmin(Filter_GetJointSessionByAdminDTO? filter, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetJointSessionsByAdmin(filter, cancellationToken);
    }

    public async Task<OperationResult<Response_GetJointStatusDTO>> GetJointStatuses(CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetJointStatuses(cancellationToken);
    }

    public async Task<OperationResult<Response_GetUnitDTO>> GetUnitsByUserId(int CustomerId, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetUnitsByUserId(CustomerId, cancellationToken);
    }

    public async Task<OperationResult<Response_GetUnitDTO>> GetUnits(CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetUnits(cancellationToken);
    }

    public async Task<OperationResult<object>> UpdateClosureJointSession(int AdminId, Request_UpdateClosureJointSessionServiceDTO model, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _reservationRepository.UpdateClosureJointSession(AdminId, new Request_UpdateClosureJointSessionDTO
            {
                Description = model.Description,
                JointSessionId = model.JointSessionId,
                Title = model.Title,
                StartTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.StartTime.Split(":")[0]), Convert.ToInt32(model.StartTime.Split(":")[1])),
                EndTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.EndTime.Split(":")[0]), Convert.ToInt32(model.EndTime.Split(":")[1])),
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return new OperationResult<object>("UpdateClosureJointSession").Failed("ساعات شروع و پایان سانس تعطیل بدرستی ارسال نشده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<object>> UpdateJoint(Request_UpdateJointDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.UpdateJoint(model, cancellationToken);
    }

    public async Task<OperationResult<object>> UpdatePrivateJointSession(int AdminId, Request_UpdatePrivateJointSessionServiceDTO model, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _reservationRepository.UpdatePrivateJointSession(AdminId, new Request_UpdatePrivateJointSessionDTO
            {
                AcceptableUnitIDs = model.AcceptableUnitIDs,
                CancellationTo = model.CancellationTo,
                Capacity = model.Capacity,
                Description = model.Description,
                EndReservationDate = model.EndReservationDate,
                GuestCapacity = model.GuestCapacity,
                JointSessionId = model.JointSessionId,
                SessionCost = model.SessionCost,
                StartReservationDate = model.StartReservationDate,
                Title = model.Title,
                UnitExtraReservationCost = model.UnitExtraReservationCost,
                UnitHasAccessForExtraReservation = model.UnitHasAccessForExtraReservation,
                StartTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.StartTime.Split(":")[0]), Convert.ToInt32(model.StartTime.Split(":")[1])),
                EndTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.EndTime.Split(":")[0]), Convert.ToInt32(model.EndTime.Split(":")[1]))
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return new OperationResult<object>("UpdatePrivateJointSession").Failed("ساعات شروع و پایان سانس خصوصی بدرستی ارسال نشده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> UpdatePublicJointSession(int AdminId, Request_UpdatePublicJointSessionServiceDTO model, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _reservationRepository.UpdatePublicJointSession(AdminId, new Request_UpdatePublicJointSessionDTO
            {
                AcceptableUnitIDs = model.AcceptableUnitIDs,
                CancellationTo = model.CancellationTo,
                Capacity = model.Capacity,
                Description = model.Description,
                EndReservationDate = model.EndReservationDate,
                GuestCapacity = model.GuestCapacity,
                JointSessionId = model.JointSessionId,
                SessionCost = model.SessionCost,
                SessionGender = model.SessionGender,
                StartReservationDate = model.StartReservationDate,
                Title = model.Title,
                UnitExtraReservationCost = model.UnitExtraReservationCost,
                UnitHasAccessForExtraReservation = model.UnitHasAccessForExtraReservation,
                StartTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.StartTime.Split(":")[0]), Convert.ToInt32(model.StartTime.Split(":")[1])),
                EndTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.EndTime.Split(":")[0]), Convert.ToInt32(model.EndTime.Split(":")[1]))
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return new OperationResult<object>("UpdatePublicJointSession").Failed("ساعات شروع و پایان سانس عمومی بدرستی ارسال نشده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_GetJointSessionByCustomerDTO>> GetJointSessionsByCustomer(int CustomerId, int UnitId, int JointId, DateTime RequestDate, Filter_GetJointSessionByCustomerDTO? filter, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetJointSessionsByCustomer(CustomerId, UnitId, JointId, RequestDate, filter, cancellationToken);
    }

    public async Task<OperationResult<Middle_GetJointSessionsByCustomerDTO>> GetSingleJointSessionByCustomer(int CustomerId, int UnitId, long JointSessionId, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetSingleJointSessionByCustomer(CustomerId, UnitId, JointSessionId, cancellationToken);
    }

    public async Task<OperationResult<Response_GetReservationsByAdminDTO>> GetReservationsByAdmin(int JointId, Filter_GetReservationsByAdminDTO? filter, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetReservationsByAdmin(JointId, filter, cancellationToken);
    }

    public async Task<OperationResult<object>> ReserveJointSessionByCustomer(Request_ReserveJointSessionServiceDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("ReserveJointSessionByCustomer");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت ثبت ارسال نشده است");
        }
        try
        {
            return await _reservationRepository.ReserveJointSessionByCustomer(new Request_ReserveJointSessionDTO
            {
                Count = model.Count,
                CustomerId = model.CustomerId,
                GuestCount = model.GuestCount,
                JointSessionId = model.JointSessionId,
                UnitId = model.UnitId,
                StartTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.StartTime.Split(":")[0]), Convert.ToInt32(model.StartTime.Split(":")[1])),
                EndTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.EndTime.Split(":")[0]), Convert.ToInt32(model.EndTime.Split(":")[1])),
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return Op.Failed("ساعت شروع یا پایان رزرو بدرستی ارسال نشده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Middle_GetReservationsByAdmin_ReservedBy>> GetCustomers(int? UnitId, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetCustomers(UnitId, cancellationToken);
    }

    public async Task<OperationResult<Response_CancelReservationDTO>> CancelReservationByAdmin(int AdminId, Request_CancelReservationDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.CancelReservationByAdmin(AdminId, model, cancellationToken);
    }

    public async Task<OperationResult<Response_GetReservationsByCustomerDTO>> GetReservationsByCustomer(int CustomerId, Filter_GetReservationsByCustomerDTO? filter, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetReservationsByCustomer(CustomerId, filter, cancellationToken);
    }

    public async Task<OperationResult<Response_CancelReservationDTO>> CancelReservationByCustomer(int CustomerId, Request_CancelReservationDTO model, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.CancelReservationByCustomer(CustomerId, model, cancellationToken);
    }

    public async Task<OperationResult<object>> CreateJointSessions(int AdminId, Request_CreateJointSessionsServiceDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateJointSessions");
        try
        {
            return await _reservationRepository.CreateJointSessions(AdminId, new Request_CreateJointSessionsDTO
            {
                AcceptableUnitIDs = model.AcceptableUnitIDs,
                CancellationTo = model.CancellationTo,
                Capacity = model.Capacity,
                Days = model.Days,
                Description = model.Description,
                EndReservationTo = model.EndReservationTo,
                GuestCapacity = model.GuestCapacity,
                HasUnitMoreReservationAccess = model.HasUnitMoreReservationAccess,
                JointId = model.JointId,
                Month = model.Month,
                PublicSessionGender = model.PublicSessionGender,
                SessionCost = model.SessionCost,
                StartReservationFrom = model.StartReservationFrom,
                Title = model.Title,
                Type = model.Type,
                UnitMoreReservationAccessCost = model.UnitMoreReservationAccessCost,
                Year = model.Year,
                Times = model.Times.Select(x => new Middle_CreateJointSessions_Times
                {
                    StartTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(x.StartTime.Split(":")[0]), Convert.ToInt32(x.StartTime.Split(":")[1])),
                    EndTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(x.EndTime.Split(":")[0]), Convert.ToInt32(x.EndTime.Split(":")[1]))
                }).ToList(),

            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return Op.Failed("بازه های زمانی بدرستی ارسال نشده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public OperationResult<Response_GetYearMonthDTO> GetYearsMonths()
    {
        return _reservationRepository.GetYearsMonths();
    }

    public async Task<OperationResult<object>> CreateSingleJointSession(int AdminId, Request_CreateSingleJointSessionServiceDTO model, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _reservationRepository.CreateSingleJointSession(AdminId, new Request_CreateSingleJointSessionDTO
            {
                AcceptableUnitIDs = model.AcceptableUnitIDs,
                CancellationTo = model.CancellationTo,
                Capacity = model.Capacity,
                Description = model.Description,
                EndReservationTo = model.EndReservationTo,
                GuestCapacity = model.GuestCapacity,
                HasUnitMoreReservationAccess = model.HasUnitMoreReservationAccess,
                JointId = model.JointId,
                PublicSessionGender = model.PublicSessionGender,
                SessionCost = model.SessionCost,
                SessionDate = model.SessionDate,
                StartReservationFrom = model.StartReservationFrom,
                Title = model.Title,
                Type = model.Type,
                UnitMoreReservationAccessCost = model.UnitMoreReservationAccessCost,
                StartTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.StartTime.Split(":")[0]), Convert.ToInt32(model.StartTime.Split(":")[1])),
                EndTime = DatetimeHelper.SetTimeSpan(Convert.ToInt32(model.EndTime.Split(":")[0]), Convert.ToInt32(model.EndTime.Split(":")[1]))
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return new OperationResult<object>("CreateSingleJointSession").Failed("ساعات سانس بدرستی ارسال نشده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetReservationReportDTO>> GetReservationReportByAdmin(int JointId, Filter_GetReservationReportDTO? filter, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetReservationReportByAdmin(JointId, filter, cancellationToken);
    }
}
