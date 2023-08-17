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
        public Player(ulong ID, string Name, List<ulong> tournaments, Dictionary<ulong, List<ulong>> pokemon, Dictionary<ulong, int> teamRolls, Dictionary<ulong, int> singleRolls) {
            this.id = ID;
            this.name = Name;
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
        public bool rollTeam(ulong tourneyId, List<Pokemon> pokemon) {
            if (!joinTournament(tourneyId)) {                
                this.teamRolls[tourneyId] -= 1;
            }
            this.pokemon[tourneyId] = new List<ulong>();
            foreach (var i in pokemon) {
                this.pokemon[tourneyId].Add(i.id);
            }
            return true;
        }

        public bool rollSingle(ulong tourneyId, ulong old, Pokemon _new) {
            if (!this.tournaments.Contains(tourneyId) ||
                !this.pokemon[tourneyId].Contains(old)) {
                return false;
            }
            this.pokemon[tourneyId].Remove(old);
            this.pokemon[tourneyId].Add(_new.id);
            this.singleRolls[tourneyId] -= 1; 
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