using Newtonsoft.Json;
using StackExchange.Redis;

using TrtShared.Envelope;
using TrtShared.ServiceCommunication;

namespace TrtUploadService.Implementation.ResultTransport
{
    public class RedisTransport : IUploadTransport
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisTransport> _logger;

        public RedisTransport(IConnectionMultiplexer redis, ILogger<RedisTransport> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task PublishPathToFileAsync(string path)
        {
            try
            {
                var pub = _redis.GetSubscriber();
                var channel = RedisChannel.Literal("file-uploaded");
                await pub.PublishAsync(channel, path);
                _logger.LogInformation("Published to Redis: {path}", path);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error! Failed to publish file path to Redis");
            }
        }

        public async Task<UniEnvelope?> GetParsedDataAsync(TimeSpan timeout)
        {
            var sub = _redis.GetSubscriber();
            var tcs = new TaskCompletionSource<UniEnvelope?>();

            var channel = RedisChannel.Literal("file-parsed");
            await sub.SubscribeAsync(channel, async (channel, message) =>
            {
                try
                {
                    if (message.IsNullOrEmpty)
                    {
                        _logger.LogError("ParserService returned empty message via Redis");
                        tcs.TrySetResult(null);
                        await sub.UnsubscribeAsync(channel);
                        return;
                    }

                    var uniEnvelope = JsonConvert.DeserializeObject<UniEnvelope?>(message!);
                    if (uniEnvelope != null)
                    {
                        tcs.TrySetResult(uniEnvelope);
                        _logger.LogInformation("Parsed usniversal envelope received from Redis with {NumItems} items", uniEnvelope.Data.Count);
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize data into universal envelope");
                        tcs.TrySetResult(null);
                    }
                    await sub.UnsubscribeAsync(channel);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical (ex, "Critical error while processing Redis message");
                    tcs.TrySetException(ex);
                    await sub.UnsubscribeAsync(channel);
                }
            });

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
            if (completed == tcs.Task)
                return await tcs.Task;
            else
            {
                _logger.LogError("Timed out waiting for parser result via Redis");
                await sub.UnsubscribeAsync(channel);
                return null;
            }
        }
    }
}
