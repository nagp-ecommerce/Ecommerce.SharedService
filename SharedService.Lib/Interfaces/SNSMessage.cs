
using System.ComponentModel.DataAnnotations;

namespace SharedService.Lib.Interfaces
{
    public record SNSMessage
    (
        [Required]
        string Type,
        [Required]
        string Message,
        string? MessageId,
        string? TopicArn,
        string? Timestamp
    );
}
