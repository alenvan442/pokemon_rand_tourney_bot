using System;
using DSharpPlus.Entities;
using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.controller;
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

        public bool rollSix(DiscordMember member, ulong tourneyId) {
            Player curr = playersFileDAO.getObject(member.Id);
            if (curr.tournaments.Contains(tourneyId) && curr.teamRolls[tourneyId] <= 0) {
                return false;
            } 
            List<Pokemon> newTeam = pokemonDAO.rollSix(member.Id);
            return curr.rollTeam(tourneyId, newTeam);
        }

        public bool rollSingle(DiscordMember member, ulong tourneyId, int selection) {
            Player curr = playersFileDAO.getObject(member.Id);
            if (!curr.tournaments.Contains(tourneyId) || curr.singleRolls[tourneyId] <= 0) {
                return false;
            }
            ulong oldId = curr.pokemon[tourneyId][selection];
            Pokemon newMon = pokemonDAO.rollOne(tourneyId, oldId, pokemonDAO.getMany(curr.pokemon[tourneyId]));
            return curr.rollSingle(tourneyId, oldId, newMon);
        }
    }
}