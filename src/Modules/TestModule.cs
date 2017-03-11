using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    class TestModule : ModuleBase
    {
        [Command("ping"), Summary("Replies with pong!")]
        public async Task Ping()
        {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync("!pong");
        }
    }
}
