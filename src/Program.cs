using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        private CommandHandler CommandHandler;
        private CommandService CommandService;
        private DiscordSocketClient Client;
        private Logger logger;

        public async Task Run()
        {
            logger = InitializeLogger();
            try
            {
                logger.Info("Loading configuration");
                var config = BotConfig.ReadConfig();

                // Initialize Client
                Client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    WebSocketProvider = () => new Win7WebSocketClient()
                });
                Client.Log += DiscordClient_Log;

                // Initialize CommandService
                CommandService = new CommandService(new CommandServiceConfig());
                await CommandService.AddModulesAsync(this.GetType().GetTypeInfo().Assembly);

                // Initialize Command Handler
                CommandHandler = new CommandHandler(Client, CommandService, config);
                Client.MessageReceived += CommandHandler.OnMessageReceived;
                Client.MessageUpdated += CommandHandler.OnMessageUpdated;

                // Start the bot
                logger.Info("Logging in and starting");
                await Client.LoginAsync(TokenType.Bot, config.Token);
                await Client.StartAsync();
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