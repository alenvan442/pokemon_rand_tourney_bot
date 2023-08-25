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
        public bool addPlayer(DiscordMember member)
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

        /// <summary>
        /// join target tournament
        /// </summary>
        /// <param name="member">the caller</param>
        /// <param name="tourneyId">the target tournament to join</param>
        /// <returns>
        ///     true: join successful
        ///     false: the user is already registered in that tournament
        /// </returns>
        public bool joinTournament(DiscordMember member, ulong tourneyId) {
            Player player = this.getObject(member.Id);
            return player.joinTournament(tourneyId);
        }

        /// <summary>
        /// set the score of a match
        /// </summary>
        /// <param name="tourneyId">the id of the tournament the match took place</param>
        /// <param name="playerOne">the first player</param>
        /// <param name="playerTwo">the second player</param>
        /// <param name="score">the result of the match in regards to player one</param>
        /// <returns>
        ///     true: set score successfully
        ///     false: the players have already fought or one or more of the players are not registered in the tournament
        /// </returns>
        public bool setScore(ulong tourneyId, ulong playerOne, ulong playerTwo, int score) {
            Player player = this.getObject(playerOne);
            Player other = this.getObject(playerTwo);
            int opponentScore = score;

            if (!(player.tournaments.Contains(tourneyId) && other.tournaments.Contains(tourneyId))) {
                return false;
            }

            if (player.alreadyFought(tourneyId, other.id) || other.alreadyFought(tourneyId, player.id)) {
                return false;
            }

            if (score == 1) {opponentScore = 0;}
            if (score == 0) {opponentScore = 1;}

            player.setScore(tourneyId, other.id, score);
            other.setScore(tourneyId, player.id, opponentScore);
            return true;
        }

        /// <summary>
        /// deletes the score between two players
        /// </summary>
        /// <param name="tourneyId">the tournament</param>
        /// <param name="playerOne">the first player</param>
        /// <param name="playerTwo">the second player</param>
        public void deleteScore(ulong tourneyId, ulong playerOne, ulong playerTwo) {
            Player first = this.getObject(playerOne);
            Player second = this.getObject(playerTwo);

            first.history[tourneyId].Remove(second.id);
            second.history[tourneyId].Remove(first.id);
        }

    }
}