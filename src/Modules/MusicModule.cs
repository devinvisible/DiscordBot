using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("music"), Alias("m"), Summary("Echos a message.")]
    public class MusicModule : ModuleBase
    {
        [Command("queue"), Alias("q")]
        public async Task QueueSongAsync(string foo)
        {
            await ReplyAsync($"test: {foo}");
        }
    }
}
