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
        //ObjectFileDAO<Events> eventsFileDAO;

        /// <summary>
        /// Constructor for the player controller
        /// Utilizes the player DAO
        /// </summary>
        /// <param name="playersFileDAO"> A class that holds methods that correspond with the manipulation of data with players </param>
        public PlayerController(PlayersFileDAO playersFileDAO, PokemonFileDAO pokemonDAO) :
                                    base(playersFileDAO)
        {
            this.playersFileDAO = playersFileDAO;
            this.pokemonDAO = pokemonDAO;
            //this.eventsFileDAO = eventsDAO;

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

        public bool joinTournament(DiscordMember member, ulong tourneyId) {
            return playersFileDAO.joinTournament(member, tourneyId);
        }

        public bool rollSix(DiscordMember member) {
            Player curr = playersFileDAO.getObject(member.Id);
            ulong tourneyId = curr.currentTournamentId;
            if (tourneyId == 0 || curr.teamRolls[tourneyId] <= 0) {
                return false;
            } 
            List<Pokemon> newTeam = pokemonDAO.rollSix(member.Id);
            return curr.rollTeam(newTeam);
        }

        public bool rollSingle(DiscordMember member, int selection) {
            Player curr = playersFileDAO.getObject(member.Id);
            ulong tourneyId = curr.currentTournamentId;
            if (tourneyId == 0 || curr.singleRolls[tourneyId] <= 0) {
                return false;
            }
            ulong oldId = curr.pokemon[tourneyId][selection];
            Pokemon newMon = pokemonDAO.rollOne(tourneyId, oldId, pokemonDAO.getMany(curr.pokemon[tourneyId]));
            return curr.rollSingle(oldId, newMon);
        }

        public bool leaveTournament(DiscordMember member, ulong tourneyId) {
            Player curr = playersFileDAO.getObject(member.Id);
            return curr.leaveTournament(tourneyId);
        }

        public List<Pokemon> viewTeam(DiscordMember caller, DiscordMember other = null) {
            Player curr = this.playersFileDAO.getObject(caller.Id);
            Player toView;
            if (other == null) {
                toView = curr;
            } else {
                toView = this.playersFileDAO.getObject(other.Id);
            }

            List<ulong> teamIds = toView.getTeam(curr.currentTournamentId);

            if (teamIds == null) {
                return null;
            }

            return this.pokemonDAO.getMany(teamIds);

        }
    }
}