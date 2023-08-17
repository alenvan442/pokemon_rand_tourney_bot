using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pokemon_rand.src.main.model.structures
{
    public class Tournament
    {
        [JsonProperty("players")]
        public List<ulong> players {get; private set;}
        [JsonProperty("host")] // this will also be the id of this tournament
        public ulong hostId {get; private set;}
        [JsonProperty("history")]
        public List<List<ulong>> history {get; private set;} // a list of records in the form of (x, 0, y)
                                                             // x is player 1, y is player 2 and the second slot
                                                             // is either a 0, 1, or 2 for lose, win, tie
                                                             // reading (x, 0, y) is x lost to y

        public Tournament(List<ulong> Players, ulong host, List<List<ulong>> history) {
            this.players = Players;
            this.hostId = host;
            this.history = history;
        }
 
        public bool removePlayer(ulong id) {
            return this.players.Remove(id);
        }

        public bool addPlayer(Player player) {
            this.players.Add(player.id);
            return true;
        }

        public bool setScore(ulong playerOne, ulong playerTwo, int score) {
            if (!this.players.Contains(playerOne) || !this.players.Contains(playerTwo)) {
                return false; //players not in tournament
            }

            if (score < 0 || score > 2) {
                return false; //invalid result
            }

            this.history.Add(new List<ulong>() {playerOne, (ulong) score, playerTwo});
            return true;
        }

        public bool deleteScore(ulong playerOne, ulong playerTwo) {
            foreach (var i in this.history) {
                if (i.Contains(playerOne) && i.Contains(playerTwo)) {
                    this.history.Remove(i);
                    return true;
                }
            }
            return false;
        }

    }
}