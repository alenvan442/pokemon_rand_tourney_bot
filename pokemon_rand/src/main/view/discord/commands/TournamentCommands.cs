using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using pokemon_rand.src.main.controller;
using pokemon_rand.src.main.model.structures;

namespace pokemon_rand.src.main.view.discord.commands
{
    public class TournamentCommands : BaseCommandModule
    {

        TournamentController tourneyController;

        public TournamentCommands(TournamentController controller) {
            this.tourneyController = controller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("host")]
        [Description("")]
        public async Task host(CommandContext ctx) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            if (!this.tourneyController.host(ctx.Member)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unable to create a new tournament: You are already hosting a tournament!");
            }

            await CommandsHelper.sendEmbed(ctx.Channel, "Succesfully created a new tournament!");
            await Task.CompletedTask;
        }

        [Command("leaderboard")]
        [Description("")]
        public async Task leaderboard(CommandContext ctx) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [Command("match")]
        [Description("")]
        public async Task match(CommandContext ctx, DiscordMember playerOne, DiscordMember playerTwo, string result) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            if (!tourneyController.isHost(ctx.Member, playerOne, playerOne)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Only the host of the tournament can use this command!");
                return;
            }

            int score = 0;
            result = result.ToLower();
            if (result == "win" || result == "won" || result == "w") {
                score = 1; 
                result = "won"; 
            }
            else if (result == "lose" || result == "lost" || result == "l") { 
                score = 0; 
                result = "lost"; 
            }
            else if (result == "tie") { 
                score = 2; 
                result = "tied";
            }
            else {
                await CommandsHelper.sendEmbed(ctx.Channel, "Invalid element inserted as the result.\n" +
                                                "Valid elements are: {win, won, w, lose, lost, l, tie}");
                return;
            }

            if (!this.tourneyController.setScore(ctx.Member, playerOne.Id, playerTwo.Id, score)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unable to set the match score: \n" +
                                    "The two players either already fought eachother, " + 
                                    "OR one or more of the players are not currently participating in this tournament.");
                return;
            }

            await CommandsHelper.sendEmbed(ctx.Channel, "Match score updated: \n" + 
                                        playerOne.Nickname + " " + result + " against " + playerTwo.Nickname);

            await Task.CompletedTask;
        }

        [Command("history")]
        [Description("")]
        public async Task history(CommandContext ctx, int pageNumber = 0) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            if (pageNumber < 0) {pageNumber = 0;}

            List<List<ulong>> scores = this.tourneyController.getHistory(ctx.Member);

            if (scores is null) {
                await CommandsHelper.sendEmbed(ctx.Channel, "You are currently not in a tournament. \n" + 
                                                "Please join a tournament first.");
                return;
            }

            // formulate the output string as pages

            await Task.CompletedTask;
        }

        [Command("tournaments")]
        [Description("")]
        public async Task tournaments(CommandContext ctx, int pageNumber) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            if (pageNumber < 0) {pageNumber = 0;}

            List<Tournament> tournaments = this.tourneyController.getObjects().ToList();

            if (tournaments is null || tournaments.Count() == 0) {
                await CommandsHelper.sendEmbed(ctx.Channel, "There are currently no active tournaments.");
                return;
            }

            // formulate the output string as pages
            
            await Task.CompletedTask;
        }

    }
}