using Discord;
using Discord.WebSocket;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        private Logger logger;

        public async Task Run()
        {
            logger = InitializeLogger();
            try
            {
                logger.Info("Loading configuration");
                var config = BotConfig.ReadConfig();

                var client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    WebSocketProvider = () => new Win7WebSocketClient()
                });
                client.Log += DiscordClient_Log;

                client.MessageReceived += async (message) =>
                {
                    if (message.Content == "!ping")
                        await message.Channel.SendMessageAsync("pong");
                };

                logger.Info("Logging in and starting");
                await client.LoginAsync(TokenType.Bot, config.Token);
                await client.StartAsync();

                logger.Info("Bot started!");
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                logger.Fatal(e);
            }
        }

        private Logger InitializeLogger()
        {
            try
            {
                var logConfig = new LoggingConfiguration();
                var consoleTarget = new ColoredConsoleTarget();

                consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} | ${message}";

                logConfig.AddTarget("Console", consoleTarget);

                logConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

                LogManager.Configuration = logConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return LogManager.GetCurrentClassLogger();
        }

        private Task DiscordClient_Log(LogMessage arg)
        {
            logger.Warn(arg.Source + " | " + arg.Message);
            if (arg.Exception != null)
                logger.Warn(arg.Exception);

            return Task.CompletedTask;
        }
    }
}