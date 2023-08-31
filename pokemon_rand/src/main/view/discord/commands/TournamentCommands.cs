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

            if (!CommandsHelper.tourneyController.host(ctx.Member)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unable to create a new tournament: You are already hosting a tournament!");
            }

            await CommandsHelper.sendEmbed(ctx.Channel, "Succesfully created a new tournament!");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [Command("leaderboard")]
        [Description("")]
        public async Task leaderboard(CommandContext ctx, int pageNumber = 1) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            List<Tuple<Player, int, int, int>> leaderboard = CommandsHelper.tourneyController.getLeaderboard(ctx.Member);
            
            string description = "";

            // set up page ranges
            int maxPages = (int)Math.Ceiling((double)leaderboard.Count / 10.0);

            if (pageNumber < 1) {pageNumber = 1;}
            if (pageNumber > maxPages) {pageNumber = maxPages;}

            int min = (pageNumber * 10) - 10;
            int max;

            if (pageNumber == maxPages) {
                max = Math.DivRem(leaderboard.Count, 10).Remainder;
            } else {
                max = 10;
            }

            int index = 10 * (pageNumber - 1);

            if (leaderboard.Count() == 0) {
                description = "This tournament currently does not have any players.";
            } else {
                leaderboard = leaderboard.GetRange(min, max);

                foreach (Tuple<Player, int, int, int> i in leaderboard) {
                    description += index + ". " + i.Item1.name + ": " + i.Item2 + "W/" + i.Item3 + "L/" + i.Item4 + "T\n";
                }
            }

            DiscordEmbedBuilder embed = CommandsHelper.createEmbed(description);
            embed.Title = "Leaderboard";
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter();
            embed.Footer.Text = "Page: " + pageNumber;

            await ctx.Channel.SendMessageAsync(embed);

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

            if (!CommandsHelper.tourneyController.isHost(ctx.Member, playerOne, playerOne)) {
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

            if (!CommandsHelper.tourneyController.setScore(ctx.Member, playerOne.Id, playerTwo.Id, score)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unable to set the match score: \n" +
                                    "The two players either already fought eachother, " + 
                                    "OR one or more of the players are not currently participating in this tournament.");
                return;
            }

            await CommandsHelper.sendEmbed(ctx.Channel, "Match score updated: \n" + 
                                        playerOne.DisplayName + " " + result + " against " + playerTwo.DisplayName);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [Command("history")]
        [Description("")]
        public async Task history(CommandContext ctx, int pageNumber = 1) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            List<string> scores = CommandsHelper.tourneyController.getHistory(ctx.Member, pageNumber);

            if (scores is null) {
                await CommandsHelper.sendEmbed(ctx.Channel, "You are currently not in a tournament. \n" + 
                                                "Please join a tournament first.");
                return;
            }

            // set up page ranges
            int maxPages = (int)Math.Ceiling((double)scores.Count / 10.0);

            if (pageNumber < 1) {pageNumber = 1;}
            if (pageNumber > maxPages) {pageNumber = maxPages;}

            int min = (pageNumber * 10) - 10;
            int max;

            if (pageNumber == maxPages) {
                max = Math.DivRem(scores.Count, 10).Remainder;
            } else {
                max = 10;
            }

            string history = "";

            if (scores.Count == 0) {
                history = "This tournament has yet to have a match completed.";
            } else {
                scores = scores.GetRange(min, max);

                foreach (string i in scores) {
                    history += i + "\n";
                }
            }

            DiscordEmbedBuilder embed = CommandsHelper.createEmbed(history);
            embed.Title = "Tournament History";
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter();
            embed.Footer.Text = "Page: " + pageNumber;

            await ctx.Channel.SendMessageAsync(embed);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [Command("tournaments")]
        [Description("")]
        public async Task tournaments(CommandContext ctx, int pageNumber = 1) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            List<string> tournaments = CommandsHelper.tourneyController.getTournaments(ctx.Member, pageNumber);

            if (tournaments is null || tournaments.Count() == 0) {
                await CommandsHelper.sendEmbed(ctx.Channel, "There are currently no active tournaments.");
                return;
            }

                        // set up page ranges
            int maxPages = (int)Math.Ceiling((double)tournaments.Count / 10.0);

            if (pageNumber < 1) {pageNumber = 1;}
            if (pageNumber > maxPages) {pageNumber = maxPages;}

            int min = (pageNumber * 10) - 10;
            int max;

            if (pageNumber == maxPages) {
                max = Math.DivRem(tournaments.Count, 10).Remainder;
            } else {
                max = 10;
            }

            tournaments = tournaments.GetRange(min, max);

            string list = "";
            foreach (string i in tournaments) {
                list += i + "\n";
            }

            DiscordEmbedBuilder embed = CommandsHelper.createEmbed(list);
            embed.Title = "All Tournaments";

            await ctx.Channel.SendMessageAsync(embed);

            // formulate the output string as pages
            
            await Task.CompletedTask;
        }

    }
}