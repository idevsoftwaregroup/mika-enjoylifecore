using Messaging.Infrastructure.Contracts.Common.Enums;

namespace Messaging.Infrastructure.Contracts.QueueMessage.Commands;

public record EnqueueMessageCommand(IEnumerable<string> RecipientKeys, DeliveryMethodType DeliveryMethod, string Body, string? Sender = null);

