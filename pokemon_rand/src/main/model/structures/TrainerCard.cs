using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pokemon_rand.src.main.model.structures
{
    public struct TrainerCard
    {
        public Player player {get; private set;}
        public Player host {get; private set;}
        public List<Pokemon> team {get; private set;}
        public int singleRolls {get; private set;}
        public int teamRolls {get; private set;}
        public List<int> record {get; private set;}
        public int ranking {get; private set;}

        public TrainerCard(Player player, Player host, List<Pokemon> team, 
                            int singleRolls, int teamRolls, List<int> record, int ranking) {

            this.player = player;
            this.host = host;
            this.team = team;
            this.singleRolls = singleRolls;
            this.teamRolls = teamRolls;
            this.record = record;
            this.ranking = ranking;

        }

    }
}