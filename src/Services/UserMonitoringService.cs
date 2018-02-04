using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    class UserMonitoringService
    {
        private readonly DiscordSocketClient _discord;
        private readonly ILogger<UserMonitoringService> _logger;

        public UserMonitoringService(DiscordSocketClient discord, ILogger<UserMonitoringService> logger)
        {
            _discord = discord;
            _logger = logger;

            _discord.CurrentUserUpdated += OnCurrentUserUpdated;

            _discord.UserJoined += OnUserJoined;
            _discord.GuildMemberUpdated += OnGuildMemberUpdated;
            _discord.UserLeft += OnUserLeft;
        }

        private async Task OnUserJoined(SocketGuildUser user)
        {
            _logger.LogInformation($"OnUserJoined {DateTime.Now} on {user.Guild.Name}: {user.Username} joined");
        }

        private async Task OnGuildMemberUpdated(SocketGuildUser before_socketGuildUser, SocketGuildUser after_socketGuildUser)
        {
            _logger.LogInformation($"OnGuildMemberUpdated {DateTime.Now} on {after_socketGuildUser.Guild.Name}: {after_socketGuildUser.Username} is {after_socketGuildUser.Status}");
        }

        private async Task OnUserLeft(SocketGuildUser user)
        {
            _logger.LogInformation($"OnUserLeft {DateTime.Now} on {user.Guild.Name}: {user.Username} left");
        }

        private async Task OnCurrentUserUpdated(SocketSelfUser socketSelfUser, SocketSelfUser socketSelfUser2)
        {
            _logger.LogInformation($"OnCurrentUserUpdated {socketSelfUser.Username}");
        }
    }
}
