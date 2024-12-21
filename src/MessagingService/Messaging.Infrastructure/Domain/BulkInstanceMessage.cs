using System.ComponentModel.DataAnnotations;

namespace Messaging.Infrastructure.Domain;

public class BulkInstanceMessage
{
    [Key]
    public int Id { get; set; }
    public string Text { get; set; }
    [Phone]
    public string To { get; set; }
    public bool Sent { get; set; }
    public Guid OperationId { get; set; }
}
