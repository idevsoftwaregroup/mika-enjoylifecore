using news.application.Contracts.DTO;

namespace news.application.Contracts.Interfaces;

public interface IMessagingService
{
    Task SendGroupSMS(List<string> phoneNumbers, string text, CancellationToken cancellationToken = default);
    Task SendNewsViaSMS(MessageNewsRequestDTO dto, CancellationToken cancellationToken = default);
}