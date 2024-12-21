using Messaging.Infrastructure.Contracts.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Infrastructure.Domain
{
    public class Message
    {
        private List<Recipient> _recipients = new();

        //public static List<string> VALID_SENDERS = new();


        public int Id { get; set; }
        public string? Sender { get; set; }
        public IEnumerable<Recipient> Recipients { get => _recipients.AsReadOnly(); set => value.DistinctBy(r => r.key).ToList(); }
        //public IEnumerable<string> RecipientKeys { get; set; }
        public DeliveryMethodType DeliveryMethod { get; set; }
        public string Body { get; set; }
        public bool Queued { get; set; }

        public void AddRecipitent(Recipient recipient)
        {
            if (_recipients.Any(r => r.key.Equals(recipient.key))) return;

            _recipients.Add(recipient);
        }

        public void UpdateRecipients(List<Recipient> messageRecipients)
        {
            _recipients = messageRecipients.DistinctBy(r => r.key).ToList();
        }
    }

    public record Recipient(string key, bool Sent, bool Delivered);
    


}
