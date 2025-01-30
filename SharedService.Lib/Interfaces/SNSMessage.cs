
namespace SharedService.Lib.Interfaces
{
    public record SNSMessage
    (
        string Type,
        string Message,
        string? MessageId,
        string? TopicArn,
        string? Timestamp
    );
}
