using Messaging.Infrastructure.Contracts.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Messaging.Infrastructure.Contracts.InstantMessage.Commands;

public class SendBulkMessageCommand
{
    public string Text { get; set; }
    //[Phone]
    public List<string> PhoneNumbers { get; set; }
    public DeliveryMethodType DeliveryMethodType { get; set; }
}
