using Messaging.API.Contracts.InstantSingleMessage;
using Messaging.API.Contracts.Lookup;
using Messaging.API.Contracts.QueuedGroupeMessage;
using Messaging.API.Contracts.QueuedSingleMessage;
using Messaging.Infrastructure.Contracts.InstantMessage.Commands;
using Messaging.Infrastructure.Contracts.Message.Commands;
using Messaging.Infrastructure.Contracts.QueueMessage.Commands;
using Messaging.Infrastructure.Services.DeliveryProviders;
using Messaging.Infrastructure.Services.MessageQueue;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.RegularExpressions;

namespace Messaging.API.Services;

public class MessagingService : IMessagingService
{
    private readonly IMessageDeliveryService _messageDeliveryService;
    private readonly IMessageQueueService _messageQueueService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public MessagingService(IMessageDeliveryService messageDeliveryService, IMessageQueueService messageQueueService, IConfiguration configuration, HttpClient httpClient)
    {
        _messageDeliveryService = messageDeliveryService;
        _messageQueueService = messageQueueService;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<InstantSingleMessageResponse> SendInstantSingleMessage(InstantSingleMessageRequest request, CancellationToken cancellationToken)
    {
        SendInstantMessageCommand command = (SendInstantMessageCommand)request;

        var result = await _messageDeliveryService.SendInstantMessageAsync(command, cancellationToken);
        return result;
    }


    public async Task<QueuedSingleMessageResponse> SendQueuedSingleMessage(QueuedSingleMessageRequest request, CancellationToken cancellationToken)
    {
        EnqueueMessageCommand command = (EnqueueMessageCommand)request;

        var result = await _messageQueueService.EnqueueMessageAsync(command, cancellationToken);

        return result;
    }


    public async Task<QueuedGroupeMessageResponse> SendQueuedGroupeMessage(QueuedGroupeMessageRequest request, CancellationToken cancellationToken)
    {
        EnqueueMessageCommand command = (EnqueueMessageCommand)request;

        var result = await _messageQueueService.EnqueueMessageAsync(command, cancellationToken);

        return result;
    }

    public async Task<SendBulkMessageResult> SendBulkMessage(SendBulkMessageCommand command, CancellationToken cancellationToken = default)
    {
        return await _messageDeliveryService.SendBulkMessageAsync(command, cancellationToken);
    }

    public async Task<SendBulkMessageResult> RetryBulkMessage(RetryBulkMessageCommand command, CancellationToken cancellationToken = default)
    {
        return await _messageDeliveryService.RetrySendBulkMessage(command, cancellationToken);
    }

    public async Task<Response_SendVerifyOtpDTO> SendVerifyOTP(Request_SendVerifyOtpDTO request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return new Response_SendVerifyOtpDTO
            {
                Success = false,
                Message = "اطلاعاتی برای ارسال پیامک اعتبارسنجی وجود ندارد",
                Status = HttpStatusCode.BadRequest
            };
        }
        if (string.IsNullOrEmpty(request.Phonenumber) || string.IsNullOrWhiteSpace(request.Phonenumber))
        {
            return new Response_SendVerifyOtpDTO
            {
                Success = false,
                Message = "ارسال شماره تماس دریافت کننده اجباری است",
                Status = HttpStatusCode.BadRequest
            };
        }
        if (!Regex.IsMatch(request.Phonenumber, "^[0-9]*$", RegexOptions.IgnoreCase) || request.Phonenumber.Length != 11 || !request.Phonenumber.StartsWith("09"))
        {
            return new Response_SendVerifyOtpDTO
            {
                Success = false,
                Message = "شماره تماس دریافت کننده بدرستی ارسال نشده است",
                Status = HttpStatusCode.BadRequest
            };
        }
        if (string.IsNullOrEmpty(request.OTP) || string.IsNullOrWhiteSpace(request.OTP))
        {
            return new Response_SendVerifyOtpDTO
            {
                Success = false,
                Message = "ارسال کد اعتبارسنجی اجباری است",
                Status = HttpStatusCode.BadRequest
            };
        }
        if (!Regex.IsMatch(request.OTP, "^[0-9]*$", RegexOptions.IgnoreCase) || request.OTP.Length != 4)
        {
            return new Response_SendVerifyOtpDTO
            {
                Success = false,
                Message = "کد اعتبارسنجی بدرستی ارسال نشده است",
                Status = HttpStatusCode.BadRequest
            };
        }
        try
        {
            var config = new LookupConfigDTO
            {
                APIKey = _configuration.GetValue<string>("SMSSettings:APIKey"),
                BaseURL = _configuration.GetValue<string>("SMSSettings:BaseURL")
            };
            if (config == null || string.IsNullOrEmpty(config.BaseURL) || string.IsNullOrWhiteSpace(config.BaseURL) || string.IsNullOrEmpty(config.APIKey) || string.IsNullOrWhiteSpace(config.APIKey))
            {
                return new Response_SendVerifyOtpDTO
                {
                    Success = false,
                    Message = "دریافت اطلاعات کاوه نگار با مشکلمواجه شده است",
                    Status = HttpStatusCode.NotFound
                };
            }
            var response = await _httpClient.GetAsync($"{config.BaseURL}/{config.APIKey}/verify/lookup.json?receptor={request.Phonenumber}&token={request.OTP}&template=enjoylifelogin", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new Response_SendVerifyOtpDTO
                {
                    Success = false,
                    Message = "ارسال پیامک اعتبارسنجی با مشکل مواجه شده است",
                    ExMessage = response.RequestMessage?.ToString(),
                    Status = response.StatusCode
                };
            }
            return new Response_SendVerifyOtpDTO
            {
                Success = true,
                Message = "ارسال پیامک اعتبارسنجی با موفقیت انجام شد",
                Status = HttpStatusCode.OK
            };

        }
        catch (Exception ex)
        {
            return new Response_SendVerifyOtpDTO
            {
                ExMessage = ex.Message,
                Message = "ارسال پیامک اعتبارسنجی با مشکل مواجه شده است",
                Status = HttpStatusCode.InternalServerError,
                Success = false
            };
        }
    }
}
