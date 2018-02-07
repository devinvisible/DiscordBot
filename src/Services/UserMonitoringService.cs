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

            _discord.GuildAvailable += OnGuildAvailable;
            _discord.JoinedGuild += OnJoinedGuild;
            _discord.LoggedIn += OnLoggedIn;
            _discord.Ready += OnReady;
            _discord.UserIsTyping += OnUserIsTyping;
            _discord.UserUpdated += OnUserUpdated;
        }

        private async Task OnUserUpdated(SocketUser before, SocketUser after)
        {
            _logger.LogInformation($"OnUserUpdated {DateTime.Now} {after.Username}");
        }

        private async Task OnUserIsTyping(SocketUser user, ISocketMessageChannel channel)
        {
            _logger.LogInformation($"OnUserIsTyping {DateTime.Now} {user.Username} is typing in {channel.Name}");
        }

        private async Task OnReady()
        {
            _logger.LogInformation($"OnReady {DateTime.Now}");
        }

        private async Task OnLoggedIn()
        {
            _logger.LogInformation($"OnLoggedIn {DateTime.Now}");
        }

        private async Task OnJoinedGuild(SocketGuild guild)
        {
            _logger.LogInformation($"OnJoinedGuild {DateTime.Now} {guild.Name}");
        }

        private async Task OnGuildAvailable(SocketGuild guild)
        {
            _logger.LogInformation($"OnGuildAvailable {DateTime.Now} {guild.Name}");
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