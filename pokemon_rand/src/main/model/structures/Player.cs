using System;
using System.Xml.Linq;
using DSharpPlus.Entities;
using pokemon_rand.src.main.model.utilities;
using Newtonsoft.Json;

namespace pokemon_rand.src.main.model.structures
{
    /// <summary>
    /// 
    /// </summary>
    public class Player
    {

        [JsonProperty("ID")]
        public ulong id { get; private set; }
        [JsonProperty("Name")]
        public string name { get; private set; }
        [JsonProperty("currentTournamentId")]
        public ulong currentTournamentId {get; private set; }
        [JsonProperty("tournaments")]
        public List<ulong> tournaments {get; private set;}

        [JsonProperty("pokemon")]
        public Dictionary<ulong, List<ulong>> pokemon {get; private set;}     //the key value is a foreign key to what tourney
        [JsonProperty("teamRolls")]
        public Dictionary<ulong, int> teamRolls {get; private set;}
        [JsonProperty("singleRolls")]
        public Dictionary<ulong, int> singleRolls {get; private set;}
        [JsonProperty("history")]
        public Dictionary<ulong, Dictionary<ulong, int>> history {get; private set;} //key is the tourneyid
                                                                                     //value is another dictionary where
                                                                                     // the key of that one is the oppinent id
                                                                                     // and the value is the result
                                                                                     // 0 for lost, 1 for win, 2 for tie.
        

        /// <summary>
        /// constructor that json uses for a new player
        /// </summary>
        /// <param name="ID">the id of the player</param>
        /// <param name="Name">the associated name of the player</param>       
        [JsonConstructor]
        public Player(ulong ID, string Name, ulong currentTournamentId, 
                        List<ulong> tournaments, Dictionary<ulong, List<ulong>> pokemon, 
                        Dictionary<ulong, int> teamRolls, Dictionary<ulong, 
                        int> singleRolls, Dictionary<ulong, Dictionary<ulong, int>> history) {
            this.id = ID;
            this.name = Name;
            this.currentTournamentId = currentTournamentId;
            this.tournaments = tournaments;
            this.pokemon = pokemon;
            this.teamRolls = teamRolls;
            this.singleRolls = singleRolls;
            this.history = history;
        }

        /// <summary>
        /// Constructor for a new player utilizing an already existing member
        /// Give them the first plant pot for free
        /// </summary>
        public Player(DiscordMember member) {
            this.id = member.Id;
            this.name = member.Username;
            this.currentTournamentId = 0;
            this.tournaments = new List<ulong>();
            this.pokemon = new Dictionary<ulong, List<ulong>>();
            this.teamRolls = new Dictionary<ulong, int>();
            this.singleRolls = new Dictionary<ulong, int>();
            this.history = new Dictionary<ulong, Dictionary<ulong, int>>();
        }

        public bool joinTournament(ulong id) {
            if (this.tournaments.Contains(id)) {
                return false;
            }
            this.tournaments.Add(id);
            this.teamRolls.Add(id, 2);
            this.singleRolls.Add(id, 2);
            this.history.Add(id, new Dictionary<ulong, int>());
            this.pokemon.Add(id, new List<ulong>());
            return true;
        }

        //check if elligible to roll will be done by caller
        public bool rollTeam(List<Pokemon> pokemon, bool force) {
            if (this.pokemon[this.currentTournamentId].Count() > 0) {
                // check if not first time (reroll)
                if (!force) {
                    this.teamRolls[this.currentTournamentId] -= 1;
                }
                this.pokemon[this.currentTournamentId].Clear();
            }

            foreach (var i in pokemon) {
                this.pokemon[this.currentTournamentId].Add(i.id);
            }
            return true;
        }

        public bool rollSingle(ulong old, Pokemon _new, bool force) {
            this.pokemon[this.currentTournamentId].Remove(old);
            this.pokemon[this.currentTournamentId].Add(_new.id);

            if (!force) {
                this.singleRolls[this.currentTournamentId] -= 1; 
            }
            return true;
        }

        public List<ulong> getTeam(ulong tourneyId = 0) {
            List<ulong> result;
            if (tourneyId == 0) {tourneyId = this.currentTournamentId;}
            bool has = this.pokemon.TryGetValue(tourneyId, out result);

            if (has == false) {
                return new List<ulong>();
            } 

            return this.pokemon[tourneyId];
        }

        public bool switchTournament(ulong tourneyId) {
            if (!this.tournaments.Contains(tourneyId)) {
                return false;
            }
            this.currentTournamentId = tourneyId;
            return true;
        }

        public bool leaveTournament(ulong tourneyId) {
            if (!this.tournaments.Contains(tourneyId)) {
                return false;
            }

            this.tournaments.Remove(tourneyId);

            // if not registered in any other tournament, then the user is not registered in any
            if (this.tournaments.Count == 0) {
                this.currentTournamentId = 0;
            }

            // if registered in more than one, set current tournament to the next tournament
            // if the current tournament was the previously deleted one
            if (this.currentTournamentId == tourneyId && this.tournaments.Count > 0) {
                this.currentTournamentId = this.tournaments[0];
            }

            return true;
        }

        public bool setScore(ulong tourneyId, ulong opponentId, int score) {
            if (score < 0 || score > 2) {
                return false; //invalid value
            }

            if (alreadyFought(tourneyId, opponentId)) {
                return false;
            }

            this.history[tourneyId].Add(opponentId, score);
            return true;
        }

        public List<int> getScore(ulong tourneyId) {
            int win = 0;
            int lose = 0;
            int tie = 0;
            Dictionary<ulong, int> curr = this.history[tourneyId];

            foreach (var i in curr.Values) {
                if (i == 0) {
                    lose++;
                } else if (i == 1) {
                    win++;
                } else if (i == 2) {
                    tie++;
                }
            }

            return new List<int>() {win, lose, tie};
        }

        public bool alreadyFought(ulong tourneyId, ulong opponentId) {
            return !this.history.ContainsKey(opponentId);
        }
        

        /// <summary>
        /// The toString method of the player class
        /// Format: 
        ///         (Player's Name)
        /// 
        ///         (Player's Balance)
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            string result = "";
            result += "\nUsername: " + this.name;

            return result;
        }

    }
}