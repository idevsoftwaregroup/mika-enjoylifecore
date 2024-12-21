using Messaging.API.Contracts.InstantSingleMessage;
using Messaging.API.Contracts.Lookup;
using Messaging.API.Contracts.QueuedGroupeMessage;
using Messaging.API.Contracts.QueuedSingleMessage;
using Messaging.API.Services;
using Messaging.Infrastructure.Contracts.InstantMessage.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Messaging.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        private readonly IMessagingService _messagingService;

        public MessagingController(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }
        [HttpPost("SendVerifyOTP")]
        public async Task<ActionResult<Response_SendVerifyOtpDTO>> SendVerifyOTP(Request_SendVerifyOtpDTO request, CancellationToken cancellationToken)
        {
            var operation = await _messagingService.SendVerifyOTP(request, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("InstantSingleMessage")]
        public async Task<ActionResult<InstantSingleMessageResponse>> SendInstantSingleMessage(InstantSingleMessageRequest request, CancellationToken cancellationToken)
        {
            return await _messagingService.SendInstantSingleMessage(request, cancellationToken);
        }

        [HttpPost("QueuedSingleMessage")]
        public async Task<ActionResult<QueuedSingleMessageResponse>> SendQueuedSingleMessage(QueuedSingleMessageRequest request, CancellationToken cancellationToken)
        {
            return await _messagingService.SendQueuedSingleMessage(request, cancellationToken);
        }

        [HttpPost("QueuedGroupeMessage")]
        public async Task<ActionResult<QueuedGroupeMessageResponse>> SendQueuedGroupeMessage(QueuedGroupeMessageRequest request, CancellationToken cancellationToken)
        {
            return await _messagingService.SendQueuedGroupeMessage(request, cancellationToken);
        }

        [HttpPost("BulkMessage")]
        public async Task<ActionResult<SendBulkMessageResult>> SendBulkMessage(SendBulkMessageCommand command, CancellationToken cancellationToken = default)
        {
            return await _messagingService.SendBulkMessage(command, cancellationToken);
        }

        [HttpPost("RetryMessage")]
        public async Task<ActionResult<SendBulkMessageResult>> RetrySendBulkMessage(RetryBulkMessageCommand command, CancellationToken cancellationToken = default)
        {
            return await _messagingService.RetryBulkMessage(command, cancellationToken);
        }


    }
}
