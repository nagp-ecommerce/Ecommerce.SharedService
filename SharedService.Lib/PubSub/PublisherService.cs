using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using SharedService.Lib.Interfaces;

namespace SharedService.Lib.PubSub
{
    public class PublisherService : IPublisherService
    {
        private IAmazonSimpleNotificationService _snsClient;
        private ILogger<PublisherService> _logger;
        public PublisherService(
            IAmazonSimpleNotificationService snsClient,
            ILogger<PublisherService> logger
        )
        {
            _snsClient = snsClient 
                ?? throw new ArgumentNullException(nameof(snsClient));
            _logger = logger;
        }

        public async Task PublishMessageAsync(string topic, object message, string subject = "")
        {
            try
            {
                var topicExists = await _snsClient.FindTopicAsync("ProductEvents");
                var topicArn = string.Empty;
                if (topicExists != null)
                {
                    topicArn = topicExists.TopicArn;
                }
                /*
                 * create new topic
                 else
                {
                    var newTopic = await _snsClient.CreateTopicAsync(topic);
                    topicArn = newTopic.TopicArn;
                }
                */

                var publishRequest = new PublishRequest
                {
                    TopicArn = topicArn,
                    Message = JsonSerializer.Serialize(message),
                };

                var res = await _snsClient.PublishAsync(publishRequest);
                
                _logger.LogInformation($"SNS Message Sent. MessageId: {res.MessageId} HttpStatusCode: {res.HttpStatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing SNS message: {ex.Message}");
            }
        }
    }
}
