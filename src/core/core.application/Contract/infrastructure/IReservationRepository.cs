using core.application.Contract.API.DTO.Reservation.Joint;
using core.application.Contract.API.DTO.Reservation.Joint.Filters;
using core.application.Contract.API.DTO.Reservation.Reservation;
using core.application.Contract.API.DTO.Reservation.Reservation.Filter;
using core.application.Framework;
using core.domain.DomainModelDTOs.ReservationDTOs;
using core.domain.entity;

namespace core.application.Contract.infrastructure
{
    public interface IReservationRepository
    {
        Task<OperationResult<Response_GetComplexDTO>> GetComplexes(CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetUnitDTO>> GetUnits(CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetUnitDTO>> GetUnitsByUserId(int CustomerId, CancellationToken cancellationToken = default);
        Task<OperationResult<Middle_GetReservationsByAdmin_ReservedBy>> GetCustomers(int? UnitId, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetJointStatusDTO>> GetJointStatuses(CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetJointDTO>> GetJoints(Filter_GetJointDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetJointDTO>> GetJoint(int JointId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateJoint(Request_CreateJointDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateJoint(Request_UpdateJointDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteJoint(Request_DeleteJointDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetJointSessionByAdminDTO>> GetJointSessionsByAdmin(Filter_GetJointSessionByAdminDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetJointSessionByCustomerDTO>> GetJointSessionsByCustomer(int CustomerId, int UnitId, int JointId, DateTime RequestDate, Filter_GetJointSessionByCustomerDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Middle_GetJointSessionsByCustomerDTO>> GetSingleJointSessionByCustomer(int CustomerId, int UnitId, long JointSessionId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateClosureJointSession(int AdminId, Request_CreateClosureJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateClosureJointSession(int AdminId, Request_UpdateClosureJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreatePrivateJointSession(int AdminId, Request_CreatePrivateJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdatePrivateJointSession(int AdminId, Request_UpdatePrivateJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreatePublicJointSession(int AdminId, Request_CreatePublicJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdatePublicJointSession(int AdminId, Request_UpdatePublicJointSessionDTO model, CancellationToken cancellationToken = default);
        OperationResult<Response_GetYearMonthDTO> GetYearsMonths();
        Task<OperationResult<object>> CreateJointSessions(int AdminId, Request_CreateJointSessionsDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateSingleJointSession(int AdminId, Request_CreateSingleJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteJointSession(Request_DeleteJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetReservationReportDTO>> GetReservationReportByAdmin(int JointId, Filter_GetReservationReportDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetReservationsByAdminDTO>> GetReservationsByAdmin(int JointId, Filter_GetReservationsByAdminDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetReservationsByCustomerDTO>> GetReservationsByCustomer(int CustomerId, Filter_GetReservationsByCustomerDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> ReserveJointSessionByCustomer(Request_ReserveJointSessionDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CancelReservationDTO>> CancelReservationByAdmin(int AdminId, Request_CancelReservationDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CancelReservationDTO>> CancelReservationByCustomer(int CustomerId, Request_CancelReservationDTO model, CancellationToken cancellationToken = default);
    }
}