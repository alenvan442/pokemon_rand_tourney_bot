using System.Globalization;
using System.Net.NetworkInformation;
using DSharpPlus.Entities;
using pokemon_rand.src.main.controller;
using pokemon_rand.src.main.model.utilities;
using Newtonsoft.Json.Serialization;
using DSharpPlus.CommandsNext;

namespace pokemon_rand.src.main.view.discord.commands
{
    public static class CommandsHelper
    {
        public static PlayerController playerController;
        public static TournamentController tourneyController;
        public static Dictionary<string, string> responses = new Dictionary<string, string> {
            {"yes", ""},
            {"no", ""}
        };

        /// <summary>
        /// Constructor of the Farming commands class
        /// </summary>
        /// <param name="plantPotController"> The controller that will be handling the delegation of plant pot interactions </param>
        public static void setup(PlayerController player, TournamentController tourney) {
            playerController = player;
            tourneyController = tourney;
        }

        public static DiscordEmbedBuilder createEmbed(string message) {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder {
                Description = message
            };
            return embed;
        }

        public static async Task sendEmbed(DiscordChannel channel, string message) {
            await channel.SendMessageAsync(createEmbed(message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static async Task<bool> callerCheck(CommandContext ctx) {
            var caller = ctx.Member;

            if (caller is null) {
                await sendEmbed(ctx.Channel, "Unknown caller error occured");
                return false;
            }

            return true;
        }

    }
}