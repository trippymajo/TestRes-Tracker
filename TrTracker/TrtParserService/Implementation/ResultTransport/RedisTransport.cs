using System.Threading.Channels;

using StackExchange.Redis;

using TrtShared.ServiceCommunication;

namespace TrtParserService.Implementation.ResultTransport
{
    class RedisTransport : IParseTransport
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisTransport> _logger;
        private readonly Channel<string> _channel;

        public RedisTransport(IConnectionMultiplexer redis, ILogger<RedisTransport> logger) 
        {
            _redis = redis;
            _logger = logger;
            _channel = Channel.CreateUnbounded<string>();

            StartListening();
        }

        public async Task PublishParsedDtoAsync(string jsonDto)
        {
            try
            {
                var pub = _redis.GetSubscriber();
                var channel = RedisChannel.Literal("file-parsed");
                await pub.PublishAsync(channel, jsonDto);
                _logger.LogInformation("Published to Redis: {dtoJson}", jsonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish dto to Redis");
            }
        }

        public ChannelReader<string> PathReader => _channel.Reader;

        /// <summary>
        /// Starts listening redis channel and pasiting info in to channel queue.
        /// Intended to run for the lifetime of the service (no unsubscription)
        /// </summary>
        private async void StartListening()
        {
            var subscriber = _redis.GetSubscriber();
            var channel = RedisChannel.Literal("file-uploaded");

            try
            {
                await subscriber.SubscribeAsync(channel, (chan, message) =>
                {
                    if (message.IsNullOrEmpty)
                    {
                        _logger.LogWarning("Received empty path");
                        return;
                    }

                    var fullFilePath = message.ToString();
                    if (string.IsNullOrWhiteSpace(fullFilePath))
                    {
                        _logger.LogWarning("Parser returned null");
                        return;
                    }

                    _logger.LogInformation("Received path: {path}", fullFilePath);
                    _channel.Writer.TryWrite(fullFilePath);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to Redis channel");
            }
        }
    }
}
