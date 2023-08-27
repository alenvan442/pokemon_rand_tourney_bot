using System.Collections.Generic;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.model.utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace pokemon_rand.src.main.model.persistence 
{
    public class TournamentFileDAO : ObjectFileDAO<Tournament> {

        public TournamentFileDAO(string jsonString, JsonUtilities jsonUtilities) : base(jsonString, jsonUtilities) { }

        /// <summary>
        /// create a new tournament
        /// </summary>
        /// <param name="hostId">the id of the host</param>
        /// <returns>
        ///     true: new tournament created successfully
        ///     false: the host is already running a different tournament
        /// </returns>
        public bool newTourney(ulong hostId) {
            if (this.getObject(hostId) != null) {
                return false;
            }

            Tournament newTourney = new Tournament(new List<ulong>(), hostId, new List<List<ulong>>());
            this.addObject(newTourney, hostId);
            save();
            return true;
        }

        /// <summary>
        /// sets the score of a match into a tournament
        /// </summary>
        /// <param name="tourneyId">the tournament that the match occured in</param>
        /// <param name="playerOne">the first player, the match is in regards to this player</param>
        /// <param name="playerTwo">the second player</param>
        /// <param name="score">the result of the match</param>
        /// <returns>
        ///     true: new match added
        ///     false: a match already exists between the two players
        /// </returns>
        public bool setScore(ulong tourneyId, ulong playerOne, ulong playerTwo, int score) {
            Tournament curr = this.getObject(tourneyId);
            bool result = curr.setScore(playerOne, playerTwo, score);
            save();
            return result;
        }

        /// <summary>
        /// deletes the score of a match from the tournament's history
        /// </summary>
        /// <param name="tourneyId">the tournament to delete from</param>
        /// <param name="playerOne">the first player</param>
        /// <param name="playerTwo">the second player</param>
        public void deleteScore(ulong tourneyId, ulong playerOne, ulong playerTwo) {
            Tournament curr = this.getObject(tourneyId);
            curr.deleteScore(playerOne, playerTwo);
            save();
        }

    }
}
