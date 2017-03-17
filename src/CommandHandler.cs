using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog;

namespace DiscordBot
{
    class CommandHandler
    {
        private readonly DiscordSocketClient Client;
        private readonly CommandService CommandService;
        private Logger Logger;
        private char Prefix;
        private DependencyMap DependencyMap;
        private BotConfig config;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, DependencyMap dependencyMap, BotConfig config)
        {
            Client = client;
            CommandService = commandService;
            DependencyMap = dependencyMap;
            Logger = LogManager.GetCurrentClassLogger();
            Prefix = config.Prefix;
        }

        internal async Task OnMessageReceived(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Ignore bot messages
            if (message.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix(Prefix, ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))) return;

            // Create a Command Context
            var context = new CommandContext(Client, message);
            
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed succesfully)
            var result = await CommandService.ExecuteAsync(context, argPos, DependencyMap);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        internal async Task OnMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
        }
    }
}
