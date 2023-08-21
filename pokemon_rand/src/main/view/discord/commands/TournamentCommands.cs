using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace pokemon_rand_tourney_bot.pokemon_rand.src.main.view.discord.commands
{
    public class TournamentCommands : BaseCommandModule
    {
        [Command("host")]
        [Description("")]
        public async Task host(CommandContext ctx) {
            await Task.CompletedTask;
        }

        [Command("leaderboard")]
        [Description("")]
        public async Task leaderboard(CommandContext ctx) {
            await Task.CompletedTask;
        }

        [Command("match")]
        [Description("")]
        public async Task match(CommandContext ctx, DiscordMember playerOne, DiscordMember playerTwo, string result) {
            await Task.CompletedTask;
        }

        [Command("history")]
        [Description("")]
        public async Task history(CommandContext ctx) {
            await Task.CompletedTask;
        }

        [Command("tournaments")]
        [Description("")]
        public async Task tournaments(CommandContext ctx) {
            await Task.CompletedTask;
        }
    }
}