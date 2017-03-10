﻿using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        public async Task Run()
        {
            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = () => new Win7WebSocketClient()
            });

            client.MessageReceived += async (message) =>
            {
                if (message.Content == "!ping")
                    await message.Channel.SendMessageAsync("pong");
            };
            
            await client.LoginAsync(TokenType.Bot, "your-bot-token");
            await client.StartAsync();
            
            await Task.Delay(-1);
        }
    }
}