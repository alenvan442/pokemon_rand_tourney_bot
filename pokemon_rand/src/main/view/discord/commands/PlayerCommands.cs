using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using pokemon_rand.src.main.controller;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.view.discord.commands;

namespace pokemon_rand_tourney_bot.pokemon_rand.src.main.view.discord.commands
{
    public class PlayerCommands : BaseCommandModule
    {

        PlayerController playerController;
        TournamentController tourneyController;
        public PlayerCommands(PlayerController controller, TournamentController tourneyController) {
            this.playerController = controller;
            this.tourneyController = tourneyController;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [Command("join")]
        [Description("")]
        public async Task join(CommandContext ctx, DiscordMember host) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            DiscordMember caller = ctx.Member;

            DiscordEmbedBuilder embed;
            if (this.playerController.joinTournament(caller, host.Id)) {
                embed = CommandsHelper.createEmbed("Sucessfully joined " + host.Nickname + "'s tournament!");
            } else {
                embed = CommandsHelper.createEmbed("You're already registered for this tournament!");
            }

            await ctx.Channel.SendMessageAsync(embed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("roll")]
        [Description("")]
        public async Task roll(CommandContext ctx) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }
            
            DiscordMember caller = ctx.Member;
            
            if (this.playerController.viewTeam(caller).Count > 0) {
                // player already has a team
                string description = "Are you sure you want to reroll your entire team?";
                if (this.interactConfirm(ctx, description).Result) {
                    if (!this.playerController.rollSix(caller)) {
                        await CommandsHelper.sendEmbed(ctx.Channel, "You do not have any team rerolls left!");
                    } else {
                        await this.team(ctx, caller, true);
                    }
                }
            } else {
                // player currently doe snot have a team
                if (!this.playerController.rollSix(caller)) {
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
        [Description("")]
        public async Task reroll(CommandContext ctx, int index = -1) {
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

                List<Pokemon> team = this.playerController.viewTeam(caller);
                if (!(team.Count > 0)) {
                    await CommandsHelper.sendEmbed(ctx.Channel, 
                            "You are currently not participating in any tournaments." +
                            "\nPlease join one using .join before attempting to reroll.");
                    return;
                }

                string targetRoll = team[index].name;
                string description = "Are you sure you want to reroll " + targetRoll + "?"; 
                if (this.interactConfirm(ctx, description).Result) {
                    if (!this.playerController.rollSingle(caller, index)) {
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
        [Description("")]
        public async Task forcereroll(CommandContext ctx, DiscordMember target, int index = -1) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            //check if host caller
            if (!this.tourneyController.isHost(ctx.Member, target)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "You are no tht ehost of the tournament, " + 
                                        "or the target player is currently not registered in your tournament");
                return;
            }

            if (!(this.playerController.viewTeam(target).Count > 0)) {
                // the player does not have a team to reroll
                await CommandsHelper.sendEmbed(ctx.Channel, target.Nickname + " does not have a team to reroll.");
                return;
            }

            if (index < 0) {
                // force full team reroll
                this.playerController.rollSix(target, true);
                await this.team(ctx, target, true);
            } else {
                if (index < 1 || index > 6 ) {
                    await CommandsHelper.sendEmbed(ctx.Channel, "Please choose a valid index between 1-6.");
                    return;
                }

                this.playerController.rollSingle(target, index, true);
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
        [Description("")]
        public async Task team(CommandContext ctx, DiscordMember target = null, bool _new = false) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            DiscordMember viewTarget = (target is null) ? ctx.Member : target;
            string description = "";
            string title = _new ?   
                                viewTarget.Nickname + "'s New Team" :
                                viewTarget.Nickname + "'s Team";

            List<Pokemon> team = this.playerController.viewTeam(viewTarget);

            for (int i = 1; i < team.Count; i++) {
                description += i + ". " + team[i-1].name + "\n";
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
        [Description("")]
        public async Task leave(CommandContext ctx, DiscordMember target) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            DiscordMember caller = ctx.Member;
            string description = "Are you sure you want to leave this tournament?";

            if (this.interactConfirm(ctx, description).Result) {
                if (this.playerController.leaveTournament(caller, target.Id)) {
                    await CommandsHelper.sendEmbed(ctx.Channel, "Left " + target + "'s tournament successfully.");
                } else {
                    await CommandsHelper.sendEmbed(ctx.Channel,
                                "You are either not currently a participant of the requested tournament, " +
                                "or that player is not hosting a tournament.");
                }
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
        [Description("")]
        public async Task _switch(CommandContext ctx, DiscordMember host) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            if (this.playerController.switchTournament(ctx.Member, host.Id)) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Switched current tournament sucessfully/");
            } else {
                await CommandsHelper.sendEmbed(ctx.Channel, "You are not registered in that tournament.");
            }

            await Task.CompletedTask;
        }

        [Command("tcard")]
        [Description("")]
        public async Task tcard(CommandContext ctx) {
            if (!CommandsHelper.callerCheck(ctx).Result) {
                return;
            }

            TrainerCard tcard = this.playerController.getTrainerCard(ctx.Member);
            
            if (tcard.player is null) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unknown error occured");
                return;
            }

            DiscordEmbedBuilder embed = CommandsHelper.createEmbed("");
            embed.Title = tcard.player.name + "'s Trainer Card";

            if (tcard.host is null) {
                embed.AddField("Tournament", "You are currently not in a tournament");
                await ctx.Channel.SendMessageAsync(embed);
                return;                
            }

            embed.AddField("Tournament", tcard.host.name);

            string teamDescription = "";

            if (tcard.team is null) {
                teamDescription = "You currently do not have a team";
            } else {
                foreach (Pokemon i in tcard.team) {
                    teamDescription += i.name + ", ";
                }
            }

            embed.AddField("Team", teamDescription);

            //guarenteed to not be null here
            embed.AddField("Single Rolls", tcard.singleRolls.ToString());
            embed.AddField("Team Rolls", tcard.teamRolls.ToString());

            // must guarentee 4 elements are in the record
            if (tcard.record is null || tcard.record.Count != 4) {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unknown error occured");
                return; 
            }

            embed.AddField("Record", tcard.record[0] + " W/" + tcard.record[1] + " L/" + 
                                        tcard.record[2] + " T/" + tcard.record[3] + " UM");
            
            embed.AddField("Ranking", tcard.ranking.ToString());
            embed.WithThumbnail(ctx.Member.AvatarUrl);

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
                
            foreach (var i in CommandsHelper.responses.Values) {
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, i));
            }

            var interactivity = ctx.Client.GetInteractivity();

            var response = await interactivity.WaitForReactionAsync(x =>
                x.User == caller &&
                x.Message == message &&
                CommandsHelper.responses.ContainsValue(x.Emoji.Name)
            );

            if (response.Result.Emoji.Name == CommandsHelper.responses["yes"]) {
                return true;
            } else if (response.Result.Emoji.Name == CommandsHelper.responses["no"]) {
                return false;
            } else {
                await CommandsHelper.sendEmbed(ctx.Channel, "Unknown interactivity error occured.");
                return false;
            }
        }

    }
}