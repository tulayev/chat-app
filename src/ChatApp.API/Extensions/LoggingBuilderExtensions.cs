using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Targets.Wrappers;

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

            var logconsole = new ColoredConsoleTarget("logconsole")
            {
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} | ${level:uppercase=true} | ${logger} | ${message} ${exception}"
            };

            logconsole.UseDefaultRowHighlightingRules = false;
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("FATAL", ConsoleOutputColor.White, ConsoleOutputColor.Red) { WholeWords = true });
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("ERROR", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange) { WholeWords = true });
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("WARN", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange) { WholeWords = true });
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("INFO", ConsoleOutputColor.Green, ConsoleOutputColor.NoChange) { WholeWords = true });

            // Keep the log file free of framework noise (ASP.NET Core, EF Core, etc.) below Warning,
            // while the console still shows everything Info+ so the app looks alive during development.
            var filteredLogfile = new FilteringTargetWrapper(logfile, ConditionParser.ParseExpression(
                "not (starts-with(logger, 'Microsoft.') or starts-with(logger, 'System.')) or level >= LogLevel.Warn"));

            logConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, filteredLogfile);
            logConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);

            LogManager.Configuration = logConfig;

            // Add NLog as Logger
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            loggingBuilder.AddNLog(logConfig);

            return loggingBuilder;
        }
    }
}
