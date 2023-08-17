using System;
using DSharpPlus.Entities;
using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.controller;

namespace pokemon_rand.src.main.controller
{
    /// <summary>
    /// This controller will receive information from the view and either add, delete, or update a player accordingly
    /// The main purpose of this controller is to receive and delegate tasks that involves the creation or deletion of a player
    /// </summary>
    public class PlayerController : ObjectController<Player>
    {
        PlayersFileDAO playersFileDAO;
        //ObjectFileDAO<Events> eventsFileDAO;

        /// <summary>
        /// Constructor for the player controller
        /// Utilizes the player DAO
        /// </summary>
        /// <param name="playersFileDAO"> A class that holds methods that correspond with the manipulation of data with players </param>
        public PlayerController(PlayersFileDAO playersFileDAO) :
                                    base(playersFileDAO)
        {
            this.playersFileDAO = playersFileDAO;
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
    }
}