using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace ServerAPI.Extensions
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddLogger(this ILoggingBuilder loggingBuilder, IConfiguration config)
        {
            var logDirSetting = config.GetValue<string>("LoggingConfig:LogDirectory");

            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            var logDir = Path.IsPathRooted(logDirSetting)
                ? logDirSetting
                : Path.Combine(projectRoot, logDirSetting!);

            Directory.CreateDirectory(logDir);

            var logConfig = new LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine(logDir, "${shortdate}.log"),
                Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message} ${exception}"
            };

            var logconsole = new ConsoleTarget("logconsole")
            {
                Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message} ${exception}"
            };

            logConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);
            logConfig.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);

            LogManager.Configuration = logConfig;

            // Add NLog as Logger
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            loggingBuilder.AddNLog(logConfig);

            return loggingBuilder;
        }
    }
}
