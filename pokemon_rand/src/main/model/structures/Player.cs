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

        

        /// <summary>
        /// constructor that json uses for a new player
        /// </summary>
        /// <param name="ID">the id of the player</param>
        /// <param name="Name">the associated name of the player</param>       
        [JsonConstructor]
        public Player(ulong ID, string Name, ulong currentTournamentId, List<ulong> tournaments, Dictionary<ulong, List<ulong>> pokemon, Dictionary<ulong, int> teamRolls, Dictionary<ulong, int> singleRolls) {
            this.id = ID;
            this.name = Name;
            this.currentTournamentId = currentTournamentId;
            this.tournaments = tournaments;
            this.pokemon = pokemon;
            this.teamRolls = teamRolls;
            this.singleRolls = singleRolls;
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
        }

        public bool joinTournament(ulong id) {
            if (this.tournaments.Contains(id)) {
                return false;
            }
            this.tournaments.Add(id);
            this.teamRolls.Add(id, 2);
            this.singleRolls.Add(id, 2);
            return true;
        }

        //check if elligible to roll will be done by caller
        public bool rollTeam(List<Pokemon> pokemon) {
            this.pokemon[this.currentTournamentId].Clear();
            foreach (var i in pokemon) {
                this.pokemon[this.currentTournamentId].Add(i.id);
            }
            return true;
        }

        public bool rollSingle(ulong old, Pokemon _new) {
            if (!this.tournaments.Contains(this.currentTournamentId) ||
                !this.pokemon[this.currentTournamentId].Contains(old)) {
                return false;
            }
            this.pokemon[this.currentTournamentId].Remove(old);
            this.pokemon[this.currentTournamentId].Add(_new.id);
            this.singleRolls[this.currentTournamentId] -= 1; 
            return true;
        }

        public List<ulong> getTeam(ulong tourneyId = 0) {
            List<ulong> result;
            if (tourneyId == 0) {tourneyId = this.currentTournamentId;}
            bool has = this.pokemon.TryGetValue(tourneyId, out result);

            if (has == false) {
                return null;
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