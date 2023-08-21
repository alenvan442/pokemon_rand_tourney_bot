using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace pokemon_rand_tourney_bot.pokemon_rand.src.main.view.discord.commands
{
    public class PlayerCommands : BaseCommandModule
    {
        [Command("join")]
        [Description("")]
        public async Task join(CommandContext ctx) {
            await Task.CompletedTask;
        }

        [Command("roll")]
        [Description("")]
        public async Task roll(CommandContext ctx) {
            await Task.CompletedTask;
        }

        [Command("reroll")]
        [Description("")]
        public async Task reroll(CommandContext ctx, int index = -1) {
            await Task.CompletedTask;
        }

        [Command("forcereroll")]
        [Description("")]
        public async Task forcereroll(CommandContext ctx, DiscordMember target, int index = -1) {
            await Task.CompletedTask;
        }

        [Command("view")]
        [Description("")]
        public async Task view(CommandContext ctx, DiscordMember target = null) {
            await Task.CompletedTask;
        }

        [Command("leave")]
        [Description("")]
        public async Task leave(CommandContext ctx) {
            await Task.CompletedTask;
        }

        [Command("switch")]
        [Description("")]
        public async Task _switch(CommandContext ctx) {
            await Task.CompletedTask;
        }

        [Command("tcard")]
        [Description("")]
        public async Task tcard(CommandContext ctx) {
            await Task.CompletedTask;
        }
    }
}