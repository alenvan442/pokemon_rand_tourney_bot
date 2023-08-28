using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pokemon_rand.src.main.model.structures
{
    public struct Pokemon
    {
        [JsonProperty("id")]
        public ulong id {get; private set;}
        [JsonProperty("name")]
        public string name {get; private set;}
        [JsonProperty("imagePath")]
        public string imagePath {get; private set;}
        [JsonProperty("evoLine")]
        public List<ulong> evoLine {get; private set;}

        public Pokemon(ulong id, string name, string imagePath, List<ulong> evoLine) {
            this.id = id;
            this.name = name;
            this.imagePath = imagePath;
            this.evoLine = evoLine;
        }

        /// <summary>
        /// check if there was a dupe accoerding to this pokemon's evolution line
        /// </summary>
        /// <param name="id">the id of the pokemon to check against</param>
        /// <returns>
        ///     true: is dupe
        ///     false: not a dupe
        /// </returns>
        public bool checkDupe(ulong id) {
            return evoLine.Contains(id);
        }
    }
}