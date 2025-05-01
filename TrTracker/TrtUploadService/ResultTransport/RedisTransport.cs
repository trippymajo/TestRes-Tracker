using Newtonsoft.Json;
using StackExchange.Redis;
using System.Diagnostics.SymbolStore;
using TrtShared.DTO;
using TrtShared.ResultTransport;

namespace TrtUploadService.ResultTransport
{
    public class RedisTransport : IResultTransport
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
                _logger.LogError(ex, "Failed to publish file path to Redis");
            }
        }

        public async Task<TestRunDTO?> GetParsedDtoAsync(TimeSpan timeout)
        {
            var sub = _redis.GetSubscriber();
            var tcs = new TaskCompletionSource<TestRunDTO?>();

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

                    var dto = JsonConvert.DeserializeObject<TestRunDTO?>(message!);
                    if (dto != null)
                    {
                        tcs.TrySetResult(dto);
                        _logger.LogInformation("Parsed DTO received from Redis: {message}", message);
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize TestRunDTO");
                        tcs.TrySetResult(null);
                    }
                    await sub.UnsubscribeAsync(channel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing Redis message");
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
