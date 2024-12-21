namespace core.infrastructure.MessagingService;

public class MessagingSettings
{
    public const string SECTION_NAME = nameof(MessagingSettings);
    public List<string> TicketSMSGroup { get; set; }
    public List<string> PaymentSMSGroup { get; set; }
    public string BaseURL { get; set; }
    public string TicketAdminTemplate { get; set; }
    public string TicketUserTemplate { get; set; }
    public string TicketMessageAdminTemplate { get; set; }
    public string TicketTrackingCodeTemplate { get; set; }
    public string TicketTrackingCodeEditTemplate { get; set; }
    public string OnlinePaymentTemplate { get; set; }
    public string OfflinePaymentTemplate { get; set; }
    public string TicketMessageTemplate { get; set; }
    public string PaymentStatusTemplate { get; set; }
    public string TicketNumberEditTemplate { get; set; }
    public string TicketClosingTemplateForCustomer { get; set; }
    public string TicketClosingTemplateForAdmin { get; set; }
    public string CreateUserUrl { get; set; }
    public string CreateUsersUrl { get; set; }
    public string UpdateUser { get; set; }
    public string DeleteUser { get; set; }
}
