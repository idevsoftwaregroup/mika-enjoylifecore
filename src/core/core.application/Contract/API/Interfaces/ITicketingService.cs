using core.application.Contract.API.DTO.Ticket;
using core.application.Contract.API.DTO.Ticket.Message;
using core.application.Framework;
using core.domain.displayEntities.ticketingModels;
using core.domain.DomainModelDTOs.TicketingDTOs;
using core.domain.DomainModelDTOs.TicketingDTOs.FilterDTOs;
using core.domain.entity.ticketingModels;
using Microsoft.AspNetCore.Http;

namespace core.application.Contract.API.Interfaces
{
    public interface ITicketingService
    {
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> CreateTicket(int UserId, CreateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> ActivateTicket(int UserId, ActivateTicketRequestDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SucceedTicket(int UserId, ModifyTicketRequestDTO requestDTO, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CloseTicketDomainDTO>> CloseTicket(int UserId, bool isAdmin, ModifyTicketRequestDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTrackCode(int UserId, UpdateTrackingCodeDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTicketNumber(int UserId, UpdateTicketNumberDTO requestDTO, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SeenTicket(int UserId, SeenTicketDTO requestDTO, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreateTicketMessageDomainDTO>> CreateMessage(int UserId, bool isAdmin, CreateMessageRequestDTO requestDTO, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetTicketDomainDTO>> GetTicketsByAdmin(int UserId, Filter_GetTicketAdminDomainDTO filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetTicketDomainDTO>> GetTickets(int UserId, Filter_GetTicketUserDomainDTO filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetSingleTicketDomainDTO>> GetTicketMessages(int UserId, bool isAdmin, long TicketId, CancellationToken cancellationToken = default);

        //Task<MessageResponseDTO> CreateMessage(CreateMessageRequestDTO requestDTO, int userId, bool isAdmin, CancellationToken cancellationToken = default);
        //Task<TicketModel> ActivateTicket(ActivateTicketRequestDTO requestDTO, int userID, CancellationToken cancellationToken = default);
        //Task<List<TicketPreviewResponseDTO>> GetAllTickets(int userID, GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default);
        //Task<List<TicketPreviewResponseDTO>> GetTicketsByUser(int userId, GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default);
        //Task<TicketDetailResponseDTO?> GetTicketById(int userId, int ticketId, CancellationToken cancellationToken = default, string sortOrder = "desc");
        //Task<TicketDetailResponseDTO?> GetTicketByIdAdmin(int userId, int ticketId, CancellationToken cancellationToken = default, string sortOrder = "desc");
        //Task<GetMessageResponseDTO> UploadAttachment(long messageId, int userId, string fileName, string contentType, Stream stream, CancellationToken cancellationToken = default);
        //Task<TicketDetailResponseDTO> UpdateTicket(int ticketId, bool isAdmin, UpdateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default);
        //Task<(List<TicketModelDisplayDTO> Tickets, int TotalCount)> GetTickets(int userId, GetAdminTicketSearchDTO requestDTO, CancellationToken cancellationToken = default);
        //Task<TicketModel> UpdateTrackCode(UpdateTrackingCodeDTO requestDTO, int userId, CancellationToken cancellationToken = default);
        //Task<int> CreateTicketVisitTime(CreateTicketVisitTimeRequestDTO requestDTO, CancellationToken cancellationToken = default);
        //Task<List<TicketVisitTimeModel>?> GetTicketsVisitTimes(GetTicketVisitSearchDTO requestDTO, CancellationToken cancellationToken = default, string sortOrder = "desc");
        //Task<int> DeleteVisitTimeById(int visitId, CancellationToken cancellationToken = default);
        //Task<TicketModel> UpdateTicketNumber(UpdateTicketNumberDTO requestDTO, int userId, CancellationToken cancellationToken = default);
        //Task<string> SeenTicket(int userId, SeenTicketDTO ticketIdDTO, CancellationToken cancellationToken = default);
        //Task<List<TicketSeen>> GetSeenedByUserId(int userId, CancellationToken cancellationToken = default);
        Task<List<string>> GetAllConnections();
        Task<List<string>> GetOtherConnections(int userId);

    }

}