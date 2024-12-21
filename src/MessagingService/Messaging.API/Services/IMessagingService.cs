using Messaging.API.Contracts.InstantSingleMessage;
using Messaging.API.Contracts.Lookup;
using Messaging.API.Contracts.QueuedGroupeMessage;
using Messaging.API.Contracts.QueuedSingleMessage;
using Messaging.Infrastructure.Contracts.InstantMessage.Commands;

namespace Messaging.API.Services
{
    public interface IMessagingService
    {
        Task<InstantSingleMessageResponse> SendInstantSingleMessage(InstantSingleMessageRequest request, CancellationToken cancellationToken);
        Task<QueuedGroupeMessageResponse> SendQueuedGroupeMessage(QueuedGroupeMessageRequest request, CancellationToken cancellationToken);
        Task<QueuedSingleMessageResponse> SendQueuedSingleMessage(QueuedSingleMessageRequest request, CancellationToken cancellationToken);
        Task<SendBulkMessageResult> SendBulkMessage(SendBulkMessageCommand command, CancellationToken cancellationToken);
        Task<SendBulkMessageResult> RetryBulkMessage(RetryBulkMessageCommand command, CancellationToken cancellationToken = default);
        Task<Response_SendVerifyOtpDTO> SendVerifyOTP(Request_SendVerifyOtpDTO request, CancellationToken cancellationToken = default);
    }
}