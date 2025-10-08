using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace ChatApp.API.Extensions
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
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} | ${level:uppercase=true} | ${logger} | ${message} ${exception}"
            };

            logConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);
            
            var logconsole = new ColoredConsoleTarget("logconsole")
            {
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} | ${level:uppercase=true} | ${logger} | ${message} ${exception}"
            };

            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("INFO", ConsoleOutputColor.Green, ConsoleOutputColor.NoChange));
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("WARN", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange));
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("ERROR", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange));
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("FATAL", ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange));

            logConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);

            LogManager.Configuration = logConfig;

            // Add NLog as Logger
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            loggingBuilder.AddNLog(logConfig);

            return loggingBuilder;
        }
    }
}
