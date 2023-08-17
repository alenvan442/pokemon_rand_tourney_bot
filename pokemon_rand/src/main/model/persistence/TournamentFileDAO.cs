using System.Collections.Generic;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.model.utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace pokemon_rand.src.main.model.persistence 
{
    public class TournamentFileDAO : ObjectFileDAO<Tournament> {

        public TournamentFileDAO(string jsonString, JsonUtilities jsonUtilities) : base(jsonString, jsonUtilities) { }

        public bool newTourney(ulong hostId) {
            if (this.getObject(hostId) != null) {
                return false;
            }

            Tournament newTourney = new Tournament(new List<ulong>(), hostId, new List<List<ulong>>());
            this.addObject(newTourney, hostId);

            return true;
        }

        public bool setScore(ulong tourneyId, ulong playerOne, ulong playerTwo, int score) {
            Tournament curr = this.getObject(tourneyId);
            return curr.setScore(playerOne, playerTwo, score);
        }

        public void deleteScore(ulong tourneyId, ulong playerOne, ulong playerTwo) {
            Tournament curr = this.getObject(tourneyId);
            curr.deleteScore(playerOne, playerTwo);
        }

    }
}
