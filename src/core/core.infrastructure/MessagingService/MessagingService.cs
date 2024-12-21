using core.application.Contract.API.DTO.Messaging;
using core.application.Contract.infrastructure.Services;
using core.application.Framework;
using core.domain.DomainModelDTOs.UserDTOs;
using IdentityProvider.Application.Helper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace core.infrastructure.MessagingService;

public class MessagingService : IMessagingService
{
    private readonly HttpClient _httpClient;
    private readonly MessagingSettings _messagingSettings;
    private readonly ILogger<MessagingService> _logger;
    public MessagingService(HttpClient httpClient, IOptions<MessagingSettings> options, ILogger<MessagingService> logger)
    {
        _httpClient = httpClient;
        _messagingSettings = options.Value;
        _logger = logger;
    }


    public async Task SendTicketAdminSMS(TicketAdminSMSDTO ticketAdminSMSDTO, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();
        replacements.Add(nameof(ticketAdminSMSDTO.Title), ticketAdminSMSDTO.Title);
        replacements.Add(nameof(ticketAdminSMSDTO.Description), ticketAdminSMSDTO.Description);
        replacements.Add(nameof(ticketAdminSMSDTO.CreatedBy), ticketAdminSMSDTO.CreatedBy);
        replacements.Add(nameof(ticketAdminSMSDTO.AttchmentUrl), ticketAdminSMSDTO.AttchmentUrl);
        replacements.Add(nameof(ticketAdminSMSDTO.CreateDate), ticketAdminSMSDTO.CreateDate);
        replacements.Add(nameof(ticketAdminSMSDTO.UnitName), ticketAdminSMSDTO.UnitName);

        string text = Regex.Replace(_messagingSettings.TicketAdminTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);

        await SendGroupSMS(_messagingSettings.TicketSMSGroup, text, cancellationToken);

    }

    public async Task SendTicketUserSMS(List<string> phoneNumbers, CancellationToken cancellationToken = default)
    {
        await SendGroupSMS(phoneNumbers, _messagingSettings.TicketUserTemplate, cancellationToken);
    }
    public async Task SendTicketMessageAdminSMS(TicketMessageAdminDTO ticketMessageAdminDTO, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();

        replacements.Add(nameof(ticketMessageAdminDTO.Title), ticketMessageAdminDTO.Title);
        replacements.Add(nameof(ticketMessageAdminDTO.Text), ticketMessageAdminDTO.Text);
        replacements.Add(nameof(ticketMessageAdminDTO.UnitName), ticketMessageAdminDTO.UnitName);
        replacements.Add(nameof(ticketMessageAdminDTO.CreatedBy), ticketMessageAdminDTO.CreatedBy);
        replacements.Add(nameof(ticketMessageAdminDTO.CreatedDate), ticketMessageAdminDTO.CreatedDate);
        replacements.Add(nameof(ticketMessageAdminDTO.TrackingCode), ticketMessageAdminDTO.TrackingCode);
        replacements.Add(nameof(ticketMessageAdminDTO.AttachmentUrl), ticketMessageAdminDTO.AttachmentUrl);

        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.TicketMessageAdminTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);
        }
        catch (Exception ex)
        {
            var uu = ex.Message;
        }


        await SendGroupSMS(_messagingSettings.TicketSMSGroup, text, cancellationToken);
    }

    public async Task SendPaymentOnlineSMS(PaymentOnlineSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();

        replacements.Add(nameof(dto.PaymenyId), dto.PaymenyId);
        replacements.Add(nameof(dto.AccountType), dto.AccountType);
        replacements.Add(nameof(dto.TotalAmount), dto.TotalAmount);
        replacements.Add(nameof(dto.CreatedDate), dto.CreatedDate);
        replacements.Add(nameof(dto.CreatedBy), dto.CreatedBy);


        string text = Regex.Replace(_messagingSettings.OnlinePaymentTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);

        await SendGroupSMS(_messagingSettings.PaymentSMSGroup, text, cancellationToken);
    }

    public async Task SendPaymentStatusSMS(PaymentStatusSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();

        replacements.Add(nameof(dto.PaymenyId), dto.PaymenyId);
        replacements.Add(nameof(dto.CreatedBy), dto.CreatedBy);
        replacements.Add(nameof(dto.AdminAction), dto.AdminAction);


        string text = Regex.Replace(_messagingSettings.PaymentStatusTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);

        await SendGroupSMS(_messagingSettings.PaymentSMSGroup, text, cancellationToken);
    }

    public async Task SendPaymentOfflineSMS(PaymentOfflineSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { nameof(dto.PaymenyId), dto.PaymenyId },
            { nameof(dto.AccountType), dto.AccountType },
            { nameof(dto.TotalAmount), dto.TotalAmount },
            { nameof(dto.BankVoucherId), dto.BankVoucherId },
            { nameof(dto.BankVoucherImage), dto.BankVoucherImage },
            { nameof(dto.CreatedDate), dto.CreatedDate },
            { nameof(dto.CreatedBy), dto.CreatedBy }
        };

        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.OfflinePaymentTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);
        }
        catch (Exception ex)
        {
            var uu = ex.Message;
        }



        await SendGroupSMS(_messagingSettings.PaymentSMSGroup, text, cancellationToken);
    }

    public async Task SendGroupSMS(List<string> phoneNumbers, string text, CancellationToken cancellationToken = default)
    {
        var body = new
        {
            Text = text,
            PhoneNumbers = phoneNumbers,
            DeliveryMethodType = 0
        };

        string json = JsonSerializer.Serialize(body);


        HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(_messagingSettings.BaseURL, httpContent, cancellationToken);

        if (!response.IsSuccessStatusCode) throw new Exception("group sms was not sent");

        var responseBody = JsonSerializer.Deserialize<BulkSMSResponse>(await response.Content.ReadAsStringAsync()) ?? throw new Exception("response could not be deserialized");

        if (!response.IsSuccessStatusCode) throw new Exception($"group sms was not sent for {responseBody.Status}"); // error handling needs some work
        //_logger.LogError(response.);
    }

    public async Task SendTicektActivationSMS(List<string> phoneNumbers, TicketActivationSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { nameof(dto.Title), dto.Title },
            { nameof(dto.TrackingCode), dto.TrackingCode },
            { nameof(dto.TicketId), dto.TicketId }
        };

        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.TicketTrackingCodeTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);
        }
        catch (Exception ex)
        {
            var uu = ex.Message;
        }


        phoneNumbers.AddRange(_messagingSettings.TicketSMSGroup);
        await SendGroupSMS(phoneNumbers, text, cancellationToken);
    }
    public async Task SendTicektUpdateSMS(List<string> phoneNumbers, TicketActivationSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { nameof(dto.Title), dto.Title },
            { nameof(dto.TrackingCode), dto.TrackingCode },
            { nameof(dto.TicketId), dto.TicketId }
        };

        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.TicketTrackingCodeEditTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);
        }
        catch (Exception ex)
        {
            var uu = ex.Message;
        }


        phoneNumbers.AddRange(_messagingSettings.TicketSMSGroup);
        await SendGroupSMS(phoneNumbers, text, cancellationToken);
    }
    public async Task SendTicketUpdateNumberSMS(List<string> phoneNumbers, TicketUpdateNumberSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { nameof(dto.Title), dto.Title },
            { nameof(dto.TicketNumber), dto.TicketNumber },
            { nameof(dto.TicketId), dto.TicketId }
        };
        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.TicketNumberEditTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);
        }
        catch (Exception ex)
        {
            var uu = ex.Message;
        }
        phoneNumbers.AddRange(_messagingSettings.TicketSMSGroup);
        await SendGroupSMS(phoneNumbers, text, cancellationToken);
    }

    public async Task SendTicketMessageSMS(string phoneNumber, TicketMessageSMSDTO ticketMessageSMSDTO, CancellationToken cancellationToken)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();

        replacements.Add(nameof(ticketMessageSMSDTO.TicketId), ticketMessageSMSDTO.TicketId);
        replacements.Add(nameof(ticketMessageSMSDTO.GenderType), ticketMessageSMSDTO.GenderType);
        replacements.Add(nameof(ticketMessageSMSDTO.TicketTitle), ticketMessageSMSDTO.TicketTitle);
        replacements.Add(nameof(ticketMessageSMSDTO.TrackingCode), ticketMessageSMSDTO.TrackingCode);
        replacements.Add(nameof(ticketMessageSMSDTO.UserFullName), ticketMessageSMSDTO.UserFullName);


        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.TicketMessageTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }


        await SendGroupSMS(new List<string>() { phoneNumber }, text, cancellationToken);
    }

    public async Task SendTicketClosingToCustomerSMS(string userPhoneNumber, TicketUpdateToCloseCustomerSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();
        replacements.Add(nameof(dto.GenderType), dto.GenderType);
        replacements.Add(nameof(dto.UserFullName), dto.UserFullName);
        replacements.Add(nameof(dto.CancelationType), dto.CancelationType);
        replacements.Add(nameof(dto.UnitName), dto.UnitName);
        replacements.Add(nameof(dto.TicketNumber), !string.IsNullOrEmpty(dto.TicketNumber) && !string.IsNullOrWhiteSpace(dto.TicketNumber) ? $"-{dto.TicketNumber}" : string.Empty);
        replacements.Add(nameof(dto.TrackingCode), dto.TrackingCode);
        replacements.Add(nameof(dto.CreatedDate), dto.CreatedDate);
        replacements.Add(nameof(dto.Title), dto.Title);
        replacements.Add(nameof(dto.CloseMessage), !string.IsNullOrEmpty(dto.CloseMessage) && !string.IsNullOrWhiteSpace(dto.CloseMessage) ? $"\n توضیحات: {dto.CloseMessage}" : string.Empty);
        replacements.Add(nameof(dto.ClosedDate), dto.ClosedDate);
        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.TicketClosingTemplateForCustomer, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        await SendGroupSMS(new List<string> { userPhoneNumber }, text, cancellationToken);

    }

    public async Task SendTicketClosingToAdminsSMS(TicketUpdateToCloseAdminSMSDTO dto, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();
        replacements.Add(nameof(dto.CancelationType), dto.CancelationType);
        replacements.Add(nameof(dto.UnitName), dto.UnitName);
        replacements.Add(nameof(dto.TicketNumber), !string.IsNullOrEmpty(dto.TicketNumber) && !string.IsNullOrWhiteSpace(dto.TicketNumber) ? $"-{dto.TicketNumber}" : string.Empty);
        replacements.Add(nameof(dto.TrackingCode), dto.TrackingCode);
        replacements.Add(nameof(dto.CreatedDate), dto.CreatedDate);
        replacements.Add(nameof(dto.Title), dto.Title);
        replacements.Add(nameof(dto.CloseMessage), !string.IsNullOrEmpty(dto.CloseMessage) && !string.IsNullOrWhiteSpace(dto.CloseMessage) ? $"\n توضیحات: {dto.CloseMessage}" : string.Empty);
        replacements.Add(nameof(dto.ClosedDate), dto.ClosedDate);
        string text = "";
        try
        {
            text = Regex.Replace(_messagingSettings.TicketClosingTemplateForAdmin, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        await SendGroupSMS(_messagingSettings.TicketSMSGroup, text, cancellationToken);
    }

    public async Task<OperationResult<object>> CreateUser(string token, int coreId, Request_CreateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateUser");
        try
        {
            var body = new
            {
                name = model.Name,
                lastname = model.Lastname,
                phoneNumber = model.PhoneNumber,
                email = model.Email,
                coreId,
                password = model.Password,
            };


            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PostAsync(_messagingSettings.CreateUserUrl, httpContent, cancellationToken);

            var responseData = await response.Content.ReadAsStringAsync(cancellationToken);
            if (responseData == null)
            {
                return Op.Failed("ثبت ساکن جدید با مشکل مواجه شده است");
            }
            var operationObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseData);
            if (operationObject == null)
            {
                return Op.Failed("ثبت ساکن جدید با مشکل مواجه شده است");
            }
            bool Success = Convert.ToBoolean(Convert.ToString(operationObject["success"] ?? "False"));
            string Message = Convert.ToString(operationObject["message"] ?? string.Empty);
            string ExMessage = Convert.ToString(operationObject["exMessage"] ?? string.Empty);
            HttpStatusCode Status = (HttpStatusCode)(Convert.ToInt32(Convert.ToString(operationObject["status"] ?? "500")));

            return Success ? Op.Succeed(Message ?? "ثبت ساکن جدید با موفقیت انجام شد", Status) : Op.Failed(Message ?? "ثبت ساکن جدید با مشکل مواجه شده است", ExMessage, Status);
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت ساکن جدید با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<object>> CreateUsers(string token, List<Response_CreatedResidentOwnerOfUnitDmainDTO> model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateUsers");
        try
        {
            var body = model.Select(x => new
            {
                name = x.Name,
                lastname = x.Lastname,
                phoneNumber = x.PhoneNumber,
                email = x.Email,
                coreId = x.CoreId
            }).ToList();

            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PostAsync(_messagingSettings.CreateUsersUrl, httpContent, cancellationToken);

            var responseData = await response.Content.ReadAsStringAsync(cancellationToken);
            if (responseData == null)
            {
                return Op.Failed("ثبت لیست ساکنین و مالکین با مشکل مواجه شده است");
            }
            var operationObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseData);
            if (operationObject == null)
            {
                return Op.Failed("ثبت لیست ساکنین و مالکین با مشکل مواجه شده است");
            }
            bool Success = Convert.ToBoolean(Convert.ToString(operationObject["success"] ?? "False"));
            string Message = Convert.ToString(operationObject["message"] ?? string.Empty);
            string ExMessage = Convert.ToString(operationObject["exMessage"] ?? string.Empty);
            HttpStatusCode Status = (HttpStatusCode)(Convert.ToInt32(Convert.ToString(operationObject["status"] ?? "500")));

            return Success ? Op.Succeed(Message ?? "ثبت لیست ساکنین و مالکین با موفقیت انجام شد", Status) : Op.Failed(Message ?? "ثبت لیست ساکنین و مالکین با مشکل مواجه شده است", ExMessage, Status);
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت لیست ساکنین و مالکین با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> UpdateUser(string token, int coreId, Request_UpdateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("UpdateUser");
        try
        {
            var body = new
            {
                name = model.Name,
                lastname = model.Lastname,
                phoneNumber = model.PhoneNumber,
                email = model.Email,
                coreId,
                password = model.Password,
            };
            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PutAsync(_messagingSettings.UpdateUser, httpContent, cancellationToken);
            var responseData = await response.Content.ReadAsStringAsync(cancellationToken);
            if (responseData == null)
            {
                return Op.Failed("بروزرسانی اطلاعات با مشکل مواجه شده است");
            }
            var operationObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseData);
            if (operationObject == null)
            {
                return Op.Failed("بروزرسانی اطلاعات با مشکل مواجه شده است");
            }
            bool Success = Convert.ToBoolean(Convert.ToString(operationObject["success"] ?? "False"));
            string Message = Convert.ToString(operationObject["message"] ?? string.Empty);
            string ExMessage = Convert.ToString(operationObject["exMessage"] ?? string.Empty);
            HttpStatusCode Status = (HttpStatusCode)(Convert.ToInt32(Convert.ToString(operationObject["status"] ?? "500")));

            return Success ? Op.Succeed(Message ?? "بروزرسانی اطلاعات با موفقیت انجام شد", Status) : Op.Failed(Message ?? "بروزرسانی اطلاعات با مشکل مواجه شده است", ExMessage, Status);
        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی اطلاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> DeleteUser(string token, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("DeleteUser");
        try
        {
            string urlWithId = _messagingSettings.DeleteUser.EndsWith("/") ? $"{_messagingSettings.DeleteUser}{model.UserId}" : $"{_messagingSettings.DeleteUser}/{model.UserId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.DeleteAsync(urlWithId, cancellationToken);
            var responseData = await response.Content.ReadAsStringAsync(cancellationToken);
            if (responseData == null)
            {
                return Op.Failed("حذف کاربر با مشکل مواجه شده است");
            }
            var operationObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseData);
            if (operationObject == null)
            {
                return Op.Failed("حذف کاربر با مشکل مواجه شده است");
            }
            bool Success = Convert.ToBoolean(Convert.ToString(operationObject["success"] ?? "False"));
            string Message = Convert.ToString(operationObject["message"] ?? string.Empty);
            string ExMessage = Convert.ToString(operationObject["exMessage"] ?? string.Empty);
            HttpStatusCode Status = (HttpStatusCode)(Convert.ToInt32(Convert.ToString(operationObject["status"] ?? "500")));

            return Success ? Op.Succeed(Message ?? "حذف کاربر با موفقیت انجام شد", Status) : Op.Failed(Message ?? "حذف کاربر با مشکل مواجه شده است", ExMessage, Status);
        }
        catch (Exception ex)
        {
            return Op.Failed("حذف کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    private class BulkSMSResponse
    {
        public Dictionary<string, bool> Status { get; set; }
    }
}
