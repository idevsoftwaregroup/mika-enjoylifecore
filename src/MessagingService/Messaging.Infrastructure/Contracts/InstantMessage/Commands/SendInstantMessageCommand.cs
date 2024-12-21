//using MediatR;
using Messaging.Infrastructure.Contracts.Common.Enums;

namespace Messaging.Infrastructure.Contracts.Message.Commands;

public record SendInstantMessageCommand(string RecipientKey, DeliveryMethodType DeliveryMethod, string Body, string? Sender=null); //: IRequest<SendInstantMessageCommandResult>;

