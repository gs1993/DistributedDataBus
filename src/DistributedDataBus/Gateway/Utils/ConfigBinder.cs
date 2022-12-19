using DataBus;

namespace Gateway.Utils
{
    public static class ConfigHelper
    {
        public static T GetConfigSection<T>(this IConfiguration configuration) where T : new()
        {
            T settings = new();
            var configSectionName = settings.GetType().Name;
            configuration.GetSection(configSectionName).Bind(settings);
            return settings;
        }
    }
}
