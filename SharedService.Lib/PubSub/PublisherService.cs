using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SharedService.Lib.Interfaces;

namespace SharedService.Lib.PubSub
{
    public class PublisherService
    {
        private IAmazonSimpleNotificationService _snsClient;
        public PublisherService(IAmazonSimpleNotificationService snsClient)
        {
            _snsClient = snsClient;
        }

        public async Task PublishMessageAsync(string topic, object message, string subject= "")
        {
            var topicExists = await _snsClient.FindTopicAsync(topic);
            var topicArn = String.Empty;
            if (topicExists != null)
            {
                topicArn = topicExists.TopicArn;
            }
            else 
            { 
                var newTopic = await _snsClient.CreateTopicAsync(topic);
                topicArn = newTopic.TopicArn;
            }

            var publishRequest = new PublishRequest { 
                TopicArn = topicArn,
                Message=JsonSerializer.Serialize(message),
                Subject = subject
            };

            await _snsClient.PublishAsync(publishRequest);
        }
    }
}
