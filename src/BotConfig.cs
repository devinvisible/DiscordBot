using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace DiscordBot
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; } = "YOUR TOKEN HERE";
        [JsonProperty("prefix")]
        public string Prefix { get; set; } = "?";

        public static BotConfig ReadConfig()
        {
            var directory = Path.GetDirectoryName(typeof(BotConfig).GetTypeInfo().Assembly.Location);
            var filepath = Path.Combine(directory, "config.json");

            if (!File.Exists(filepath))
            {
                File.WriteAllText(filepath, JsonConvert.SerializeObject(new BotConfig()));
                throw new FileNotFoundException($"Config File Not Found - Generating a template at '{filepath}'. Put your bot token in here.");
            }

            var jsonString = File.ReadAllText(filepath);

            return JsonConvert.DeserializeObject<BotConfig>(jsonString);
        }
    }
}
