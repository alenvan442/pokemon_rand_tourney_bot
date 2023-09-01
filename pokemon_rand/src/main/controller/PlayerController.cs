using System;
using DSharpPlus.Entities;
using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;
using pokemon_rand_tourney_bot.pokemon_rand.src.main.model.persistence;

namespace pokemon_rand.src.main.controller
{
    /// <summary>
    /// This controller will receive information from the view and either add, delete, or update a player accordingly
    /// The main purpose of this controller is to receive and delegate tasks that involves the creation or deletion of a player
    /// </summary>
    public class PlayerController : ObjectController<Player>
    {
        PlayersFileDAO playersFileDAO;
        PokemonFileDAO pokemonDAO;
        TournamentFileDAO tournamentDAO;
        TournamentController tournamentController;
        //ObjectFileDAO<Events> eventsFileDAO;

        /// <summary>
        /// Constructor for the player controller
        /// Utilizes the player DAO
        /// </summary>
        /// <param name="playersFileDAO"> A class that holds methods that correspond with the manipulation of data with players </param>
        public PlayerController(PlayersFileDAO playersFileDAO, PokemonFileDAO pokemonDAO, TournamentFileDAO tournamentDAO,
                                TournamentController tournamentController) :
                                    base(playersFileDAO)
        {
            this.playersFileDAO = playersFileDAO;
            this.pokemonDAO = pokemonDAO;
            this.tournamentDAO = tournamentDAO;
            this.tournamentController = tournamentController;
        }

        /// <summary>
        /// Adds a new player to the database
        /// </summary>
        /// <param name="member"> The discord member that will gain a new acconut </param>
        /// <returns> A boolean indicating whether or not the action was successful </returns>
        public bool addPlayer(DiscordMember member)
        {
            return playersFileDAO.addPlayer(member);
        }

        /// <summary>
        /// adds a player to a tournament
        /// </summary>
        /// <param name="member">player to join</param>
        /// <param name="tourneyId">id of the tournament to join</param>
        /// <returns>
        ///     true: join successfully, 
        ///     false: already joined that tournament
        /// </returns>
        public bool joinTournament(DiscordMember member, ulong tourneyId) {
            return playersFileDAO.joinTournament(member, tourneyId);
            // TODO do an override for the host in case people are allowed to join back in due to a mistake
        }

        /// <summary>
        /// rolls 5 random pokemon and 1 random mythical/legend
        /// and adds them to the user
        /// </summary>
        /// <param name="member">the playe rthat is rolling</param>
        /// <param name="force">whether or not this was a force roll invoked by the host</param>
        /// <returns>
        ///     true: roll and add was successful
        ///     false: the user either is not in a tournament or does not have any more free rolls left 
        /// </returns>
        public bool rollSix(DiscordMember member, bool force = false) {
            // force will only ever be true in a force reroll, this will not be set in the front end
            Player curr = playersFileDAO.getObject(member.Id);
            ulong tourneyId = curr.currentTournamentId;
            if (tourneyId == 0 || (!force && curr.teamRolls[tourneyId] <= 0)) {
                return false;
            } 
            List<Pokemon> newTeam = pokemonDAO.rollSix();
            bool result = curr.rollTeam(newTeam, force);
            this.playersFileDAO.save();
            return result;
        }

        /// <summary>
        /// roll a single pokemon
        /// </summary>
        /// <param name="member">player who is rolling</param>
        /// <param name="selection">which index of the pokemon list to reroll</param>
        /// <param name="force">whether invoked by host</param>
        /// <returns>
        ///     true: roll was successful
        ///     false: the user either is not in a tournament or does not have any more free rolls left 
        /// </returns>
        public bool rollSingle(DiscordMember member, int selection, bool force = false) {
            // force will only ever be true in a force reroll, this will not be set in the front end
            Player curr = playersFileDAO.getObject(member.Id);
            ulong tourneyId = curr.currentTournamentId;
            if (tourneyId == 0 || (!force && curr.singleRolls[tourneyId] <= 0)) {
                return false;
            }
            ulong oldId = curr.pokemon[tourneyId][selection-1];
            Pokemon newMon = pokemonDAO.rollOne(oldId, pokemonDAO.getMany(curr.pokemon[tourneyId]));
            bool result = curr.rollSingle(oldId, newMon, force);
            this.playersFileDAO.save();
            return result;
        }

        /// <summary>
        /// leave a selected tournament
        /// </summary>
        /// <param name="member">leaving player</param>
        /// <param name="tourneyId">which tournament to leave</param>
        /// <returns>
        ///     true: leave success
        ///     false: player is currently not registered in that tournament
        /// </returns>
        public bool leaveTournament(DiscordMember member, ulong tourneyId) {
            Player curr = playersFileDAO.getObject(member.Id);
            bool result = curr.leaveTournament(tourneyId);
            if (result) {
                List<ulong> playerIds = this.tournamentController.getObject(tourneyId).players;
                foreach (var i in playerIds) {
                    Player player = playersFileDAO.getObject(i);
                    player.removeRecords(tourneyId, curr.id);
                }
                this.playersFileDAO.save();
            }
            return result;
        }

        /// <summary>
        /// view the team of a player
        /// </summary>
        /// <param name="caller">the player invoking the command</param>
        /// <param name="other">the target of the command, defaults to the caller</param>
        /// <returns>
        ///     The list of pokemon that the target has, will be empty if they have yet to roll
        /// </returns>
        public List<Pokemon> viewTeam(DiscordMember caller, DiscordMember other = null) {
            Player curr = this.playersFileDAO.getObject(caller.Id);
            Player toView;
            if (other == null) {
                toView = curr;
            } else {
                toView = this.playersFileDAO.getObject(other.Id);
            }

            if (toView.currentTournamentId == 0) {return null;}

            List<ulong> teamIds = toView.getTeam(curr.currentTournamentId);

            if (teamIds == null) {
                return null;
            }

            return this.pokemonDAO.getMany(teamIds);
        }
        

        /// <summary>
        /// HOST USE ONLY
        /// will force a reroll on that intended target
        /// </summary>
        /// <param name="target">target player to invoke the roll</param>
        /// <param name="rollTarget">the index of the player's team to roll (single reroll), otherwise roll all (team reroll)</param>
        /// <returns>
        ///     true: roll was successful
        ///     false: the player is not registered in the tournament 
        /// </returns>
        public bool forceReroll(DiscordMember target, int rollTarget = 0) {
            if (rollTarget == 0) {
                // force roll all
                return this.rollSix(target, true);
            } else {
                return this.rollSingle(target, rollTarget, true);
            }

        }

        public TrainerCard getTrainerCard(DiscordMember member) {
            Player player = this.getObject(member.Id);
            if (player.currentTournamentId == 0) {return new TrainerCard();}
            // get host
            Player host = this.getObject(player.currentTournamentId);
            // get team
            List<Pokemon> team = this.pokemonDAO.getMany(player.getTeam());
            // get rolls left
            int singleRolls = player.singleRolls[player.currentTournamentId];
            int teamRolls = player.teamRolls[player.currentTournamentId];
            // get current score and standing
            List<int> score = this.tournamentController.getPlayerScore(member);
            List<Tuple<Player, int, int, int>> leaderboard = this.tournamentController.getLeaderboard(member);
            int ranking = 0;

            for (int i = 0; i < leaderboard.Count(); i++) {
                if (member.Id == leaderboard[i].Item1.id) {
                    ranking = i + 1;
                }
            }
            //int ranking 
            return new TrainerCard(player, host, team, singleRolls, teamRolls, score, ranking);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="tourneyId"></param>
        /// <returns></returns>
        public bool switchTournament(DiscordMember member, ulong tourneyId) {
            bool result = this.playersFileDAO.getObject(member.Id).switchTournament(tourneyId);
            this.playersFileDAO.save();
            return result;
        }

    }
}