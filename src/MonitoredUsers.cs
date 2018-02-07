using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class MonitoredUsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public int DiscordId { get; set; }
        public string UserName { get; set; }
    }

    public class Guild
    {
        public int GuildId { get; set; }
        public int DiscordGuildId { get; set; }
        public string GuildName { get; set; }
    }

    public enum Status
    {
        Offline,
        Idle,
        Online,
        Typing,
        SentMessage
    }

    public class GuildMemberUpdate
    {
        public int GuildMemberUpdateId { get; set; }
        public Guild Guild { get; set; }
        public User User { get; set; }
        public Status Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
