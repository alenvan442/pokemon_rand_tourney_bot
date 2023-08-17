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
        public Dictionary<ulong, Player> players {get; private set;}
        [JsonProperty("host")] // this will also be the id of this tournament
        public ulong hostId {get; private set;}

        public Tournament(Dictionary<ulong, Player> Players, ulong host) {
            this.players = Players;
            this.hostId = host;
        }

        public bool removePlayer(ulong id) {
            return this.players.Remove(id);
        }

        public bool addPlayer(Player player) {
            this.players.Add(player.id, player);
            return true;
        }

        public Player getPlayer(ulong id) {
            Player result;
            this.players.TryGetValue(id, out result);
            return result;
        }

    }
}