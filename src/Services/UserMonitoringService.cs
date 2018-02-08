using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    class UserMonitoringService
    {
        private readonly DiscordSocketClient _discord;
        private readonly ILogger<UserMonitoringService> _logger;
        private readonly MonitoredUsersContext _db;

        public UserMonitoringService(DiscordSocketClient discord, ILogger<UserMonitoringService> logger, MonitoredUsersContext db)
        {
            _discord = discord;
            _logger = logger;
            _db = db;

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

        private Task OnUserUpdated(SocketUser before, SocketUser after)
        {
            _logger.LogInformation($"OnUserUpdated {DateTime.Now} {after.Username}");
            return Task.CompletedTask;
        }

        private Task OnUserIsTyping(SocketUser user, ISocketMessageChannel channel)
        {
            _logger.LogInformation($"OnUserIsTyping {DateTime.Now} {user.Username} is typing in {channel.Name}");
            return Task.CompletedTask;
        }

        private Task OnReady()
        {
            _logger.LogInformation($"OnReady {DateTime.Now}");
            return Task.CompletedTask;
        }

        private Task OnLoggedIn()
        {
            _logger.LogInformation($"OnLoggedIn {DateTime.Now}");
            return Task.CompletedTask;
        }

        private Task OnJoinedGuild(SocketGuild guild)
        {
            _logger.LogInformation($"OnJoinedGuild {DateTime.Now} {guild.Name}");
            return Task.CompletedTask;
        }

        private Task OnGuildAvailable(SocketGuild guild)
        {
            _logger.LogInformation($"OnGuildAvailable {DateTime.Now} {guild.Name}");
            return Task.CompletedTask;
        }

        private Task OnUserJoined(SocketGuildUser user)
        {
            _logger.LogInformation($"OnUserJoined {DateTime.Now} on {user.Guild.Name}: {user.Username} joined");
            return Task.CompletedTask;
        }

        private async Task OnGuildMemberUpdated(SocketGuildUser before_socketGuildUser, SocketGuildUser after_socketGuildUser)
        {
            _logger.LogInformation($"OnGuildMemberUpdated {DateTime.Now} on {after_socketGuildUser.Guild.Name}: {after_socketGuildUser.Username} is {after_socketGuildUser.Status}");


            var user = _db.Users.SingleOrDefault(u => u.DiscordId == after_socketGuildUser.Id);
            user.UserName = after_socketGuildUser.Username;

            var guild = _db.Guilds.SingleOrDefault(g => g.DiscordGuildId == after_socketGuildUser.Guild.Id);
            guild.GuildName = after_socketGuildUser.Guild.Name;

            Status status = Status.Offline;
            switch (after_socketGuildUser.Status)
            {
                case Discord.UserStatus.Online: status = Status.Online; break;
                case Discord.UserStatus.Offline: status = Status.Offline; break;
                case Discord.UserStatus.Idle: status = Status.Idle; break;
            }

            //_db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //_db.Entry(guild).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _db.Updates.AddAsync(new GuildMemberUpdate()
            {
                Guild = guild,
                User = user,
                Status = status,
                Timestamp = DateTimeOffset.Now
            });
            await _db.SaveChangesAsync();
        }

        private Task OnUserLeft(SocketGuildUser user)
        {
            _logger.LogInformation($"OnUserLeft {DateTime.Now} on {user.Guild.Name}: {user.Username} left");
            return Task.CompletedTask;
        }

        private Task OnCurrentUserUpdated(SocketSelfUser socketSelfUser, SocketSelfUser socketSelfUser2)
        {
            _logger.LogInformation($"OnCurrentUserUpdated {socketSelfUser.Username}");
            return Task.CompletedTask;
        }
    }
}