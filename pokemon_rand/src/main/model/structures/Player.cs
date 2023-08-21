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

        /// <summary>
        /// join target tournament and initialize default objects for the tournament
        /// </summary>
        /// <param name="id">the tournament to join</param>
        /// <returns>
        ///     true: join success
        ///     false: player is already registered in this tournament
        /// </returns>
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

        /// <summary>
        /// sets a full team of pokemon for the player
        /// check if elligible to roll will be done by caller function
        /// </summary>
        /// <param name="pokemon">a list of 6 pokemon to set the team as</param>
        /// <param name="force">whether or not this was a force roll invoked by the host</param>
        /// <returns>
        ///     true: added successful
        /// </returns>
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

        /// <summary>
        /// replaces a single pokemon in the player's team
        /// </summary>
        /// <param name="old">the pokemon to replace</param>
        /// <param name="_new">the nee pokemon</param>
        /// <param name="force">whether this was a forced roll by the host</param>
        /// <returns>
        ///     true: added successfilly
        /// </returns>
        public bool rollSingle(ulong old, Pokemon _new, bool force) {
            this.pokemon[this.currentTournamentId].Remove(old);
            this.pokemon[this.currentTournamentId].Add(_new.id);

            if (!force) {
                this.singleRolls[this.currentTournamentId] -= 1; 
            }
            return true;
        }

        /// <summary>
        /// gets the team of the player
        /// </summary>
        /// <param name="tourneyId">the tournament to view the player's team of</param>
        /// <returns>
        ///     list of 6 pokemon ids
        /// </returns>
        public List<ulong> getTeam(ulong tourneyId = 0) {
            List<ulong> result;
            if (tourneyId == 0) {tourneyId = this.currentTournamentId;}
            bool has = this.pokemon.TryGetValue(tourneyId, out result);

            if (has == false) {
                return new List<ulong>();
            } 

            return this.pokemon[tourneyId];
        }

        /// <summary>
        /// switch what tournament the player is currently viewing
        /// </summary>
        /// <param name="tourneyId">the tournament to switch to</param>
        /// <returns>
        ///     true: switch success
        ///     false: the player is not registered in the targer tournament
        /// </returns>
        public bool switchTournament(ulong tourneyId) {
            if (!this.tournaments.Contains(tourneyId)) {
                return false;
            }
            this.currentTournamentId = tourneyId;
            return true;
        }

        /// <summary>
        /// leave target tournament
        /// if it's the current tournament then either reset
        /// the player to not have a tournament OR
        /// the player's current tournament being the next registered tournament
        /// </summary>
        /// <param name="tourneyId">the tournament to leave</param>
        /// <returns>
        ///     true: leave success
        ///     false: the playe ris not registered in that tournament 
        /// </returns>
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

        /// <summary>
        /// sets the score between this player and an opponent
        /// </summary>
        /// <param name="tourneyId">the tournament the match was in</param>
        /// <param name="opponentId">the opponent</param>
        /// <param name="score">the result of the match</param>
        /// <returns>
        ///     true: add success
        ///     false: invalid score
        /// </returns>
        public bool setScore(ulong tourneyId, ulong opponentId, int score) {
            if (score < 0 || score > 2) {
                return false; //invalid value
            }

            this.history[tourneyId].Add(opponentId, score);
            return true;
        }

        /// <summary>
        /// get the score of the player in a specific tournament
        /// </summary>
        /// <param name="tourneyId">the tournament to get the score from</param>
        /// <returns>
        ///     the list of scores in the format of (wins, losts, ties)
        /// </returns>
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

        /// <summary>
        /// check whether or not the player has already fought the opponent
        /// </summary>
        /// <param name="tourneyId">the tournament to check in</param>
        /// <param name="opponentId">the opponent</param>
        /// <returns>
        ///     true: already fought the opponent
        ///     false: have yet to fight the opponent
        /// </returns>
        public bool alreadyFought(ulong tourneyId, ulong opponentId) {
            return this.history.ContainsKey(opponentId);
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