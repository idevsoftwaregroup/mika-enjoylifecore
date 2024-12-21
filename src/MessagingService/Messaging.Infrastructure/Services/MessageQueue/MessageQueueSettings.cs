using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Infrastructure.Services.MessageQueue
{
    public class MessageQueueSettings
    {
        public const string SECTION_NAME = nameof(MessageQueueSettings);

        public string UriString { get; set; } = null!;
        public string ClientProvidedName { get; set; } = null!;
        public string ExchangeName { get; set; } = null!;
        public string RoutingKey { get; set; } = null!;
        public string QueueName { get; set; } = null!;


    }
}
