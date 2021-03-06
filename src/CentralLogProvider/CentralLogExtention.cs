using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CentralLogProvider {
    public static class CentralLogExtensions {
        public static ILoggingBuilder AddCentralLog(this ILoggingBuilder builder, CentralLogOptions options) {
            builder.Services.AddSingleton<CentralLogOptions>(options);
            builder.Services.AddSingleton<ILoggerProvider, CentralLogProvider>();
            return builder;
        }
    }
}