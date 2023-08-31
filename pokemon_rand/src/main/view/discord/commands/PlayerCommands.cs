using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using pokemon_rand.src.main.controller;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.view.discord.commands;

namespace pokemon_rand_tourney_bot.pokemon_rand.src.main.view.discord.commands
{
    public class PlayerCommands : BaseCommandModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [Command("join")]
        [Description("Join a target tournament.")]
        public async Task join(CommandContext ctx, 
                                [Description("The host of the tournament to join: input the name of the host.")]
                                DiscordMember host) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            DiscordMember caller = ctx.Member;
            DiscordEmbedBuilder embed;
            if (CommandsHelper.tourneyController.join(caller.Id, host.Id)) { 
                if (CommandsHelper.playerController.joinTournament(caller, host.Id)) {
                    embed = CommandsHelper.createEmbed("Sucessfully joined " + host.DisplayName + "'s tournament!");
                } else {
                    CommandsHelper.tourneyController.leave(caller.Id, host.Id, false);
                    embed = CommandsHelper.createEmbed("You're already registered for this tournament!");
                }
            } else {
                embed = CommandsHelper.createEmbed("You're either already registered for this tournament, " +
                                                    "\nrecently left this tournament and can not re-join, " +
                                                    "\nor the tournament does not exist.");
            }
            await ctx.Channel.SendMessageAsync(embed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="target"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [Command("forcejoin")]
        [Description("HOST ONLY Force a player to join a tournament.")]
        public async Task forcejoin(CommandContext ctx, 
                                    [Description("The target player to join: input the name of the player.")]
                                    DiscordMember target,
                                    [Description("The host of the tournament to join: input the name of the host.")]
                                    DiscordMember host) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            //check if host caller
            if (!CommandsHelper.tourneyController.isHost(ctx.Member, host.Id)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "You are not the host of the tournament, ");
                return;
            }

            DiscordEmbedBuilder embed;
            if (CommandsHelper.tourneyController.join(target.Id, host.Id, true)) { 
                if (CommandsHelper.playerController.joinTournament(target, host.Id)) {
                    embed = CommandsHelper.createEmbed("Sucessfully joined " + host.DisplayName + "'s tournament!");
                } else {
                    CommandsHelper.tourneyController.leave(target.Id, host.Id, false);
                    embed = CommandsHelper.createEmbed("You're already registered for this tournament!");
                }
            } else {
                embed = CommandsHelper.createEmbed("You're either already registered for this tournament.");
            }
            await ctx.Channel.SendMessageAsync(embed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("roll")]
        [Description("Roll or reroll your whole team.")]
        public async Task roll(CommandContext ctx) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }
            
            DiscordMember caller = ctx.Member;
            
            if (CommandsHelper.playerController.viewTeam(caller).Count > 0) {
                // player already has a team
                string description = "Are you sure you want to reroll your entire team?";
                if (this.interactConfirm(ctx, description).Result) {
                    if (!CommandsHelper.playerController.rollSix(caller)) {
                        await CommandsHelper.sendEmbed(ctx.Channel, "You do not have any team rerolls left!");
                    } else {
                        await this.team(ctx, caller, true);
                    }
                }
            } else {
                // player currently doe snot have a team
                if (!CommandsHelper.playerController.rollSix(caller)) {
                    await CommandsHelper.sendEmbed(ctx.Channel, 
                            "You are currently not participating in any tournaments." +
                            "\nPlease join one using .join before rolling a team.");
                } else {
                    await this.team(ctx, caller, true);
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Command("reroll")]
        [Description("Reroll either your whole team or a single selection.")]
        public async Task reroll(CommandContext ctx, 
                                    [Description("The index between 1 and 6 of which pokemon you want to reroll. " +
                                    "Leave this empty if you want to reroll the whole team")]
                                    int index = -1) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            DiscordMember caller = ctx.Member;

            if (index < 0) {
                // this defaults to a team reroll, let the roll command handle this
                await this.roll(ctx);
            } else {
                if (index < 1 || index > 6) {
                    await CommandsHelper.sendEmbed(ctx.Channel, "Please choose a valid index between 1-6.");
                    return;
                }

                List<Pokemon> team = CommandsHelper.playerController.viewTeam(caller);
                if (!(team.Count > 0)) {
                    await CommandsHelper.sendEmbed(ctx.Channel, 
                            "You are currently not participating in any tournaments." +
                            "\nPlease join one using .join before attempting to reroll.");
                    return;
                }

                string targetRoll = team[index-1].name;
                string description = "Are you sure you want to reroll " + targetRoll + "?"; 
                if (this.interactConfirm(ctx, description).Result) {
                    if (!CommandsHelper.playerController.rollSingle(caller, index)) {
                        await CommandsHelper.sendEmbed(ctx.Channel, "You do not have any single rerolls left!");
                    } else {
                        await this.team(ctx, caller, true);
                    }
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="target"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Command("forcereroll")]
        [Description("HOST ONLY Force a reroll on a target player.")]
        public async Task forcereroll(CommandContext ctx,
                                        [Description("The target player to reroll: input the name of the player.")]
                                        DiscordMember target,
                                        [Description("The index between 1 and 6 of which pokemon you want to reroll. " +
                                        "Leave this empty if you want to reroll the whole team")]
                                        int index = -1) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            //check if host caller
            if (!CommandsHelper.tourneyController.isHost(ctx.Member, target)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "You are not the host of the tournament, ");
                return;
            }

            if (!(CommandsHelper.playerController.viewTeam(target).Count > 0)) {
                // the player does not have a team to reroll
                await CommandsHelper.sendEmbed(ctx.Channel, target.DisplayName + " does not have a team to reroll.");
                return;
            }

            if (index < 0) {
                // force full team reroll
                if (!CommandsHelper.playerController.rollSix(target, true)) {
                    await CommandsHelper.sendEmbed(ctx.Channel, "Player: " + target.DisplayName + " is currently not registered in your tournament.");
                    return;
                }
                await this.team(ctx, target, true);
            } else {
                if (index < 1 || index > 6 ) {
                    await CommandsHelper.sendEmbed(ctx.Channel, "Please choose a valid index between 1-6.");
                    return;
                }

                if (!CommandsHelper.playerController.rollSingle(target, index, true)) {
                    await CommandsHelper.sendEmbed(ctx.Channel, "Player: " + target.DisplayName + " is currently not registered in your tournament.");
                    return;
                }
                await this.team(ctx, target, true);
            } 

            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="target"></param>
        /// <param name="_new"></param>
        /// <returns></returns>
        [Command("team")]
        [Description("View you team for your current tournament.")]
        public async Task team(CommandContext ctx, 
                                [Description("Which player's team in your current tournament would you like to view, leave empty for yourself. " +
                                "Input the name of the player.")]
                                DiscordMember target = null, 
                                [Description("Disregard this.")]
                                bool _new = false) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            DiscordMember viewTarget = (target is null) ? ctx.Member : target;
            string description = "";
            string title = _new ?   
                                viewTarget.DisplayName + "'s New Team" :
                                viewTarget.DisplayName + "'s Team";

            List<Pokemon> team = CommandsHelper.playerController.viewTeam(viewTarget);

            if (team.Count == 0 ) {
                description = "You currently do not have a team, use .roll to get your first team.";
            } else if (team.Count != 6) {
                description = "Unknown error occured. Team rolled did not contain 6 pokemon.";
            } else {
                for (int i = 1; i <= team.Count; i++) {
                    description += i + ". " + team[i-1].name + "\n";
                }
            }

            DiscordEmbedBuilder embed = CommandsHelper.createEmbed(description);
            embed.Title = title;

            await ctx.Channel.SendMessageAsync(embed);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [Command("leave")]
        [Description("Leave a target tournament.")]
        public async Task leave(CommandContext ctx, 
                                [Description("Which tournament you would like to leave: input the name of the host.")]
                                DiscordMember target) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            DiscordMember caller = ctx.Member;
            string description = "Are you sure you want to leave this tournament?";

            if (this.interactConfirm(ctx, description).Result) {
                string feedback = "";
                if (CommandsHelper.tourneyController.leave(caller.Id, target.Id)) {

                    if (CommandsHelper.playerController.leaveTournament(caller, target.Id)) {

                        feedback = "Left " + target.DisplayName + "'s tournament successfully.";
                    } else {
                        feedback = "You are either not currently a participant of the requested tournament, " +
                                    "or that player is not hosting a tournament.";
                    }
                } else {
                    feedback = "You are either not currently a participant of the requested tournament, " +
                                    "or that player is not hosting a tournament.";
                }
                await CommandsHelper.sendEmbed(ctx.Channel, feedback);
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [Command("switch")]
        [Description("Switch between tournaments.")]
        public async Task _switch(CommandContext ctx, 
                                    [Description("The target tournament to switch to: input th ename of the host.")]
                                    DiscordMember host) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            if (CommandsHelper.playerController.switchTournament(ctx.Member, host.Id)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Switched current tournament sucessfully.");
            } else {
                await CommandsHelper.sendEmbed(ctx.Channel, "You are not registered in that tournament.");
            }

            await Task.CompletedTask;
        }

        [Command("tcard")]
        [Description("View your trainer card for your current tournament.")]
        public async Task tcard(CommandContext ctx) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            TrainerCard tcard = CommandsHelper.playerController.getTrainerCard(ctx.Member);

            DiscordEmbedBuilder embed = CommandsHelper.createEmbed("");
            embed.Title = tcard.player is null ? ctx.Member.DisplayName : tcard.player.name;
            embed.Title += "'s Trainer Card";
            embed.WithThumbnail(ctx.Member.AvatarUrl);

            if (tcard.player is null || tcard.host is null) {
                embed.AddField("Tournament", "You are currently not in a tournament");
                await ctx.Channel.SendMessageAsync(embed);
                return;                
            }

            embed.AddField("Tournament", tcard.host.name);

            string teamDescription = "";

            if (tcard.team is null || tcard.team.Count == 0) {
                teamDescription = "You currently do not have a team";
            } else {
                for (int i = 0; i < tcard.team.Count; i++) {
                    if (i == tcard.team.Count() - 1) {
                        teamDescription += tcard.team[i].name;
                    } else {
                        teamDescription += tcard.team[i].name + ", ";
                    }
                }
            }

            embed.AddField("Team", teamDescription);

            // must guarentee 4 elements are in the record
            if (tcard.record.Count != 4) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unknown error occured");
                return; 
            }

            embed.AddField("Ranking", tcard.ranking.ToString());

            if (tcard.record is null) { // could happen sometimes, not too sure why
                embed.AddField("Record", "There is no one else in this tournament.");
            } else {
                embed.AddField("Record", tcard.record[0] + " W/" + tcard.record[1] + " L/" + 
                                            tcard.record[2] + " T/" + tcard.record[3] + " UM");
            }

            //guarenteed to not be null here
            embed.AddField("Single Rolls", tcard.singleRolls.ToString());
            embed.AddField("Team Rolls", tcard.teamRolls.ToString());

            await ctx.Channel.SendMessageAsync(embed);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private async Task<bool> interactConfirm(CommandContext ctx, string description) {
            DiscordMember caller = ctx.Member;
            var message = await ctx.Channel.SendMessageAsync(CommandsHelper.createEmbed(description));
            
            List<DiscordEmoji> emojis = new List<DiscordEmoji>();
            foreach (var i in CommandsHelper.responses.Values) {
                DiscordEmoji curr = DiscordEmoji.FromName(ctx.Client, i);
                emojis.Add(curr);
                await message.CreateReactionAsync(curr);
            }

            var interactivity = ctx.Client.GetInteractivity();

            var response = await interactivity.WaitForReactionAsync(x =>
                x.User == ctx.User &&
                x.Message == message &&
                (x.Emoji == emojis[0] ||
                 x.Emoji == emojis[1])
            );

            if (response.Result.Emoji == emojis[0]) {
                return true;
            } else if (response.Result.Emoji == emojis[1]) {
                return false;
            } else {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unknown interactivity error occured.");
                return false;
            }
        }

    }
}