using System.Globalization;
using System.Net.NetworkInformation;
using DSharpPlus.Entities;
using pokemon_rand.src.main.controller;
using pokemon_rand.src.main.model.utilities;
using Newtonsoft.Json.Serialization;

namespace pokemon_rand.src.main.view.discord.commands
{
    public static class CommandsHelper
    {
        public static PlayerController playerController;
        //public static ObjectController<> employeeController;

        /// <summary>
        /// Constructor of the Farming commands class
        /// </summary>
        /// <param name="plantPotController"> The controller that will be handling the delegation of plant pot interactions </param>
        public static void setup(PlayerController player) {

            playerController = player;
        }

        public static DiscordEmbedBuilder createEmbed(string message) {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder {
                Description = message
            };
            return embed;
        }

    }
}