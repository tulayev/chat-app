using ChatApp.Application.Mappings;
using Mapster;

namespace ChatApp.Tests.TestHelpers
{
    public static class MapsterTestConfig
    {
        public static readonly TypeAdapterConfig Instance = BuildConfig();

        private static TypeAdapterConfig BuildConfig()
        {
            var config = new TypeAdapterConfig();
            config.Scan(typeof(UserMapping).Assembly);
            return config;
        }
    }
}
