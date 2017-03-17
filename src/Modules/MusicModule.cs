using Discord;
using Discord.Audio;
using Discord.Commands;
using DiscordBot.Service.Music;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("music"), Alias("m"), Summary("Echos a message.")]
    public class MusicModule : ModuleBase
    {
        private readonly AudioService audioService;

        public MusicModule(AudioService service)
        {
            audioService = service;
        }

        // You *MUST* mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Alias("j")]
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("You aren't in a voice channel so you need to mention which voice channel to join");
                return;
            }

            await audioService.JoinAudio(Context.Guild, channel);
        }

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await audioService.LeaveAudio(Context.Guild);
        }

        [Alias("q")]
        [Command("queue", RunMode = RunMode.Async)]
        public async Task QueueSongAsync([Remainder]string foo)
        {
            await ReplyAsync($"test: {foo}");
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            //song = @"E:\Ratatat - Black Heroes [HD].mp3";
            await audioService.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        [Alias("seen", "?")]
        [Command("last", RunMode = RunMode.Async)]
        public async Task LastSongAsync()
        {
            string song = "?";



            await ReplyAsync($"The last song I see is `{song}`");
        }
    }
}
