using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Threading.Tasks;

public interface IRedisService
{
    Task<string> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
    Task SetExpirationAsync(string key, TimeSpan expiration);
}

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisService(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("Redis:ConnectionString").Value;
        _redis = ConnectionMultiplexer.Connect(connectionString);
    }

    public async Task<string> GetValueAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.StringGetAsync(key);
    }

    public async Task SetValueAsync(string key, string value)
    {
        var db = _redis.GetDatabase();
        await db.StringSetAsync(key, value);
    }

    public async Task SetExpirationAsync(string key, TimeSpan expiration)
    {
        var db = _redis.GetDatabase();
        await db.KeyExpireAsync(key, expiration);
    }
}
