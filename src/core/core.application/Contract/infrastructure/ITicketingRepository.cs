using core.application.Contract.API.DTO.Ticket;
using core.application.Framework;
using core.domain.displayEntities.ticketingModels;
using core.domain.DomainModelDTOs.TicketingDTOs;
using core.domain.DomainModelDTOs.TicketingDTOs.FilterDTOs;
using core.domain.entity.ticketingModels;

namespace core.application.Contract.infrastructure
{
    public interface ITicketingRepository
    {
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> CreateTicket(int UserId, Request_CreateTicketDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> ActivateTicket(int UserId, Request_ActivateTicketDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SucceedTicket(int UserId, Request_ModifyTicketDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CloseTicketDomainDTO>> CloseTicket(int UserId, bool isAdmin, Request_ModifyTicketDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTrackCode(int UserId, Request_UpdateTrackCodeDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTicketNumber(int UserId, Request_UpdateTicketNumberDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SeenTicket(int UserId, Request_TicketId model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreateTicketMessageDomainDTO>> CreateMessage(int UserId, bool isAdmin, Request_CreateTicketMessageDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetTicketDomainDTO>> GetTicketsByAdmin(int UserId, Filter_GetTicketAdminDomainDTO filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetTicketDomainDTO>> GetTickets(int UserId, Filter_GetTicketUserDomainDTO filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetSingleTicketDomainDTO>> GetTicketMessages(int UserId, bool isAdmin, long TicketId, CancellationToken cancellationToken = default);


        //Task<TicketModel?> GetTicketByIdAsync(int ticketId, int userId = 0, CancellationToken cancellationToken = default);
        //Task<TicketMessageModel?> GetMessageByIdAsync(long messageId, CancellationToken cancellationToken = default);
        //Task<List<TicketMessageModel>> GetTicketMessages(int TicketId, CancellationToken cancellationToken = default, string sortOrder = "desc");
        //Task<List<TicketModel>> GetTicketsByUser(int userId, GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default);
        //Task<TicketModel> SaveTicketAsync(TicketModel ticket, CancellationToken cancellationToken = default);
        //Task<TicketMessageModel> SaveTicketMessageAsync(TicketMessageModel ticketMessage, CancellationToken cancellationToken = default);
        //Task<TicketModel> UpdateTicketAsync(TicketModel ticketModel, bool modifyByAdmin, string? closeMessage, CancellationToken cancellationToken = default);
        //Task<TicketMessageModel> UpdateMessageAsync(TicketMessageModel ticketMessageModel, CancellationToken cancellationToken = default);
        ////Task<List<TicketModel>?> GetAllTicketAsync(GetTicketSearchDTO requestDTO, CancellationToken cancellationToken = default);
        //Task<(List<TicketModelDisplayDTO> Tickets, int TotalCount)> GetAllTicketAsync(int userId,GetAdminTicketSearchDTO requestDTO, CancellationToken cancellationToken = default);
        //Task<int> SaveTicketVisitTimeAsync(TicketVisitTimeModel TicketVisitTime, CancellationToken cancellationToken = default);
        //Task<List<TicketVisitTimeModel>>? GetTicketsVisitTimesAsync(GetTicketVisitSearchDTO requestDTO, CancellationToken cancellationToken = default);
        //Task<int> DeleteVisitTimeByIdAsync(int visitId, CancellationToken cancellationToken = default);
        //string GetTicketStatusDisplayTitle(TicketStatus status);
        //string GetTicketStatusName(TicketStatus status);
        //int GetTicketStatusCode(TicketStatus status);
        //Task<string> SeenTicket(int userId, int ticketId, CancellationToken cancellationToken = default);
        //Task<List<TicketSeen>> GetSeenedByUserId(int userId, CancellationToken cancellationToken = default);

    }
}