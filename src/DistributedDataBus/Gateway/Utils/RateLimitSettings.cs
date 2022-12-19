using AspNetCoreRateLimit;

namespace Gateway.Utils
{
    public class RedisSettings
    {
        public string Url { get; init; }
        public string Password { get; init; }

        public string ConnectionString => $"{Url},password={Password}";
    }
}
