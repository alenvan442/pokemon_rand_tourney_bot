using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.model.utilities;

namespace pokemon_rand_tourney_bot.pokemon_rand.src.main.model.persistence
{
    public class PokemonFileDAO : ObjectFileDAO<Pokemon>
    {

        public Dictionary<ulong, Pokemon> basics;
        public Dictionary<ulong, Pokemon> legends;
        public PokemonFileDAO(string jsonString, JsonUtilities jsonUtilities) : base(jsonString, jsonUtilities) {
            this.legends = new Dictionary<ulong, Pokemon>();
            initCategories();
        }

        private void initCategories() {
            List<ulong> ids = new List<ulong>();
            this.basics = this.data;

            foreach (var i in ids) {
                Pokemon temp;
                this.data.TryGetValue(i, out temp);
                if (temp != null) {
                    this.basics.Remove(i);
                    this.legends.Add(i, temp);
                }
            }
        }

        //use player id as the seed
        public List<Pokemon> rollSix(ulong seed) {
            List<Pokemon> result = new List<Pokemon>();
            Random rand = new Random((int) seed);
            Pokemon temp;
            List<Pokemon> basics = this.basics.Values.ToList();
            List<Pokemon> legends = this.legends.Values.ToList();

            while (result.Count < 5) {
                int i = rand.Next(this.basics.Count);
                temp = basics[i];
                if (checkDupe(result, temp)) {
                    result.Add(temp);
                }
            }

            while (result.Count < 6) {
                int i = rand.Next(this.legends.Count);
                temp = legends[i];
                if (checkDupe(result, temp)) {
                    result.Add(temp);
                }
            }
            return result;
        }

        //use player id as the seed
        public Pokemon rollOne(ulong seed, ulong old, List<Pokemon> currTeam) {
            Pokemon result = null;
            Random rand = new Random((int) seed);
            int max = 0;
            List<Pokemon> list;

            if (this.basics.ContainsKey(old)) {
                max = this.basics.Count;
                list = this.basics.Values.ToList();
            } else if (this.legends.ContainsKey(old)) {
                max = this.legends.Count;
                list = this.legends.Values.ToList();
            } else {
                return null;
            }

            while (result == null || checkDupe(currTeam, result)) {
                int i = rand.Next(max);
                result = list[i];
            }

            return result;
        }

        public bool checkDupe(List<Pokemon> currTeam, Pokemon checkMon) {
            foreach (var i in currTeam) {
                if (i.checkDupe(checkMon.id)) {
                    return false;
                }
            }
            return true;
        }
    }
}