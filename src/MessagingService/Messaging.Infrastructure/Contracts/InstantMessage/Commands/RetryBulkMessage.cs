using Messaging.Infrastructure.Contracts.Common.Enums;

namespace Messaging.Infrastructure.Contracts.InstantMessage.Commands;

public class RetryBulkMessageCommand
{
    public DeliveryMethodType DeliveryMethod { get; set; }
 
}
