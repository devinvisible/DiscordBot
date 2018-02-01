
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new Program().MainAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
        }

        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Discord
            services.AddSingleton(_client);
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandlingService>();

            // Logging
            services.AddLogging();
            services.AddSingleton<LogService>();

            // Configuration
            services.AddSingleton(_config);

            return services.BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            var directory = Path.GetDirectoryName(typeof(BotConfig).GetTypeInfo().Assembly.Location);
            var filepath = Path.Combine(directory, "config.json");

            if (!File.Exists(filepath))
            {
                var defaultConfig = new
                {
                    token = "YOUR TOKEN HERE",
                    prefix = "?"
                };

                File.WriteAllText(filepath, JsonConvert.SerializeObject(defaultConfig));
                throw new FileNotFoundException($"Config file not found. Generating a template at '{filepath}'. Put your bot token in here.");
            }

            Console.WriteLine($"Reading configuration from: {filepath}");
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(filepath)
                .Build();
        }
    }
}