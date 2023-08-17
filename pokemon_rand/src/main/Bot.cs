using System.Diagnostics.Tracing;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using pokemon_rand.src.main.model.utilities;
using pokemon_rand.src.main.view.discord.commands;

/// <summary>
/// This is the class that holds the Discord Bot configuration
/// </summary>
namespace pokemon_rand.src.main
{
    public class Bot
    {
        //declare the client and commands extenstions
        public DiscordClient client { get; set; }
        public CommandsNextExtension commands { get; set; }

        //upon bot startup, do this
        public async Task RunAsync()
        {

            LoadDAO.load();

            //create the configuration of the bot
            var clientConfig = new DiscordConfiguration
            {
                AutoReconnect = true,
                Intents = DiscordIntents.All,
                Token = StaticUtil.token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            };

            //set the client's configs
            client = new DiscordClient(clientConfig);
            client.GuildAvailable += OnGuildAvailable;
            client.Ready += OnReady;

            //initialize the commands configurations
            var commandConfig = new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                IgnoreExtraArguments = true,
                StringPrefixes = new string[] { ".", "/" },

            };

            //set the commands of the bot
            commands = client.UseCommandsNext(commandConfig);
            //commands.RegisterCommands<>();

            //Set up interactivity
            client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //connect the bot to the client
            await client.ConnectAsync();
            await Task.Delay(-1);

        }

        /// <summary>
        /// This function fires once the client is ready
        /// </summary>
        /// <param name="client"> the client that is ready </param>
        /// <param name="e"> the args that are passed in once the client is ready </param>
        /// <returns> a task </returns>
        public async Task OnReady(DiscordClient client, ReadyEventArgs e)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// This function fires once the client joins a guild and recognizes the guild
        /// </summary>
        /// <param name="client"> the client that has joined a guild </param>
        /// <param name="e"> the args that are passed in once the client connects to a guild </param>
        /// <returns> a task </returns>
        public async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            LoadDAO.load();
            await Task.CompletedTask;
        }


    }
}