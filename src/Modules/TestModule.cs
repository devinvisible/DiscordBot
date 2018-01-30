using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping"), Summary("Replies with pong!")]
        public async Task Ping()
        {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync("!pong");
        }


        [Command("test")]
        public async Task Test()
        {
            var message = await Context.Channel.SendMessageAsync("Testing");

            var skipRateLimit = new RequestOptions();
            skipRateLimit.BypassBuckets = true;

            await message.AddReactionAsync(new Emoji("\u0031\u20e3"), skipRateLimit);
            await message.AddReactionAsync(new Emoji("\u0032\u20e3"), skipRateLimit);
            await message.AddReactionAsync(new Emoji("\u0033\u20e3"), skipRateLimit);
        }
    }
}