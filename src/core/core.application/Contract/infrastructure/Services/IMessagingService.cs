using core.application.Contract.API.DTO.Messaging;
using core.application.Framework;
using core.domain.DomainModelDTOs.UserDTOs;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace core.application.Contract.infrastructure.Services;

public interface IMessagingService
{
    Task SendGroupSMS(List<string> phoneNumbers, string text, CancellationToken cancellationToken = default);
    Task SendPaymentOfflineSMS(PaymentOfflineSMSDTO dto, CancellationToken cancellationToken = default);
    Task SendPaymentOnlineSMS(PaymentOnlineSMSDTO dto, CancellationToken cancellationToken = default);
    Task SendTicketAdminSMS(TicketAdminSMSDTO ticketAdminSMSDTO, CancellationToken cancellationToken = default);
    Task SendTicketMessageAdminSMS(TicketMessageAdminDTO ticketMessageAdminDTO, CancellationToken cancellationToken = default);
    Task SendTicketUserSMS(List<string> phoneNumbers, CancellationToken cancellationToken = default);
    Task SendTicektActivationSMS(List<string> phoneNumbers, TicketActivationSMSDTO ticketActivationSMSDTO, CancellationToken cancellationToken = default);
    Task SendTicektUpdateSMS(List<string> phoneNumbers, TicketActivationSMSDTO dto, CancellationToken cancellationToken = default);
    Task SendTicketMessageSMS(string phoneNumber, TicketMessageSMSDTO ticketMessageSMSDTO, CancellationToken cancellationToken);
    Task SendPaymentStatusSMS(PaymentStatusSMSDTO dto, CancellationToken cancellationToken = default);
    Task SendTicketUpdateNumberSMS(List<string> phoneNumbers, TicketUpdateNumberSMSDTO dto, CancellationToken cancellationToken = default);
    Task SendTicketClosingToCustomerSMS(string userPhoneNumber, TicketUpdateToCloseCustomerSMSDTO dto, CancellationToken cancellationToken = default);
    Task SendTicketClosingToAdminsSMS(TicketUpdateToCloseAdminSMSDTO dto, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> CreateUser(string token, int coreId, Request_CreateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> CreateUsers(string token, List<Response_CreatedResidentOwnerOfUnitDmainDTO> model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> UpdateUser(string token, int coreId, Request_UpdateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteUser(string token, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
}
