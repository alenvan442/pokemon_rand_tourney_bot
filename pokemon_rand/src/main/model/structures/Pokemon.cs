using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pokemon_rand.src.main.model.structures
{
    public class Pokemon
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

        public bool checkDupe(ulong id) {
            if (evoLine.Contains(id)) {
                return true;
            }
            return false;
        }
    }
}