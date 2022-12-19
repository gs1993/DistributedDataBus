namespace Contracts.Utils
{
    public sealed class GrpcSettings
    {
        public GrpcConnectionSetting[] Connections { get; init; }

        public string GetUrl(string name)
        {
            return Connections.Single(x => x.Name == name).Url;
        }
    }

    public sealed class GrpcConnectionSetting
    {
        public string Name { get; init; }
        public string Url { get; init; }
    }
}
