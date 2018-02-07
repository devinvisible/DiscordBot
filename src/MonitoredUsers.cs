using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot
{
    public class MonitoredUsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<GuildMemberUpdate> Updates { get; set; }

        public MonitoredUsersContext(DbContextOptions<MonitoredUsersContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=monitoring.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuildMemberUpdate>()
                .Property(u => u.Timestamp)
                .HasDefaultValueSql("getdate()");
        }
    }

    public class User
    {
        [Key]
        public int DiscordId { get; set; }
        public string UserName { get; set; }
    }

    public class Guild
    {
        [Key]
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
        public Guild Guild { get; set; }
        public User User { get; set; }
        public Status Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
