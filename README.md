# DiscordBot
This is a discord bot. It's for educational purposes. It almost certainely breaks API ToS. Don't actually use this.

```
git clone https://github.com/devinvisible/DiscordBot.git
cd DiscordBot
git submodule update --init --recursive
```
```
cd src
dotnet run
```
It will fail throwing an AggregateException/FileNotFoundException and generate `bin\netcoreapp2.0\config.json`.
Grab your bot token (NOT client ID) from https://discordapp.com/developers/applications/me and put it in the config.json file.
```
dotnet run
```
