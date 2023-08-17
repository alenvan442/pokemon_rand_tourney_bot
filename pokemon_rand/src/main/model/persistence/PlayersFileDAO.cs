using System;
using DSharpPlus.Entities;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.model.utilities;

namespace pokemon_rand.src.main.model.persistence
{
    /// <summary>
    /// This DAO file will handle all player data in terms of loading, saving, creating, and deleting player data
    /// </summary>
    public class PlayersFileDAO : ObjectFileDAO<Player>
    {


        /// <summary>
        /// Constructor for the PlayersFileDAO object
        /// setups the DAO object
        /// loads the player datas
        /// </summary>
        /// <param name="playersJson"> The path to the json file </param>
        /// <param name="jsonUtilities"> An object that helps with json manipulations </param>
        public PlayersFileDAO(string playersJson, JsonUtilities jsonUtilities) : base(playersJson, jsonUtilities) { }

        /// <summary>
        /// Adds a new player
        /// Creates a new player and adds them to the local collection then saves
        /// </summary>
        /// <param name="member"> The discord member to add </param>
        /// <returns> A boolean to indicate whether or not the player was successfully created </returns>
        public Boolean addPlayer(DiscordMember member)
        {
            //Checks to see if the UID is already in the system
            if (this.data.TryGetValue(member.Id, out _))
            {
                return false;
            }
            else
            {
                Player newPlayer = new Player(member);
                addObject(newPlayer, member.Id);
                return true;
            }
        }

        public bool joinTournament(DiscordMember member, ulong tourneyId) {
            Player player = this.getObject(member.Id);
            return player.joinTournament(tourneyId);
        }

    }
}