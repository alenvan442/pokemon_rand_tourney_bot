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
            this.basics = new Dictionary<ulong, Pokemon>(this.data);
            this.legends = new Dictionary<ulong, Pokemon>();
            initCategories();
        }

        /// <summary>
        /// initializes the categories of pokemon
        /// </summary>
        private void initCategories() {
            List<ulong> ids = new List<ulong>() {144, 145, 146, 150, 243, 244, 245, 249, 250, 377,
                                                378, 379, 380, 381, 382, 383, 384, 480, 481, 482, 
                                                483, 484, 485, 486, 487, 488, 638, 639, 640, 641, 
                                                642, 643, 644, 645, 646, 716, 717, 718, 772, 773, 
                                                785, 786, 787, 788, 789, 790, 791, 792, 800, 888, 
                                                889, 890, 891, 892, 894, 895, 896, 897, 898, 905,
                                                1001, 1002, 1003, 1004, 1007, 1008, 1009, 1010,
                                                151, 251, 385, 386, 489, 490, 491, 492, 493, 494, 
                                                647, 648, 649, 719, 720, 721, 801, 802, 807, 808, 
                                                809, 893}; // write this to a file later

            foreach (var i in ids) {
                Pokemon temp;
                this.data.TryGetValue(i, out temp);
                if (temp != null) {
                    this.basics.Remove(i);
                    this.legends.Add(i, temp);
                }
            }
        }

        /// <summary>
        /// given a list of pokemon ids, return the corresponding pokemon
        /// </summary>
        /// <param name="ids">list of pokemon ids</param>
        /// <returns>
        ///     a list of pokemon
        /// </returns>
        public List<Pokemon> getMany(List<ulong> ids) {
            List<Pokemon> result = new List<Pokemon>();
            foreach(var i in ids) {
                result.Add(this.getObject(i));
            }
            return result;
        }

        /// <summary>
        /// roll 6 pokemon, 5 basics, 1 legend
        /// duplicates are not allowed
        /// </summary>
        /// <param name="seed">the randomizer seed</param>
        /// <returns>
        ///     a list of 6 pokemon, 5 basics, 1 legendary
        /// </returns>
        public List<Pokemon> rollSix() {
            List<Pokemon> result = new List<Pokemon>();
            Random rand = new Random();
            Pokemon temp;
            List<Pokemon> basics = this.basics.Values.ToList();
            List<Pokemon> legends = this.legends.Values.ToList();

            while (result.Count < 5) {
                int i = rand.Next(this.basics.Count);
                temp = basics[i];
                if (!checkDupe(result, temp)) {
                    result.Add(temp);
                }
            }

            while (result.Count < 6) {
                int i = rand.Next(this.legends.Count);
                temp = legends[i];
                if (!checkDupe(result, temp)) {
                    result.Add(temp);
                }
            }
            return result;
        }

        /// <summary>
        /// Roll a single selection, used as a reroll for a team
        /// duplicates are not allowed
        /// </summary>
        /// <param name="seed">the randomizer seed</param>
        /// <param name="old">the id of the pokemon to replace</param>
        /// <param name="currTeam">the current team of the caller</param>
        /// <returns>
        ///     The pokemon to replace the old one with
        /// </returns>
        public Pokemon rollOne(ulong old, List<Pokemon> currTeam) {
            Pokemon result = null;
            Random rand = new Random();
            int max;
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

        /// <summary>
        /// check for a duplicate pokemon
        /// </summary>
        /// <param name="currTeam">the team to check the pokemon against</param>
        /// <param name="checkMon">the pokemon to check against the team</param>
        /// <returns>
        ///     true: there was a dupe
        ///     false: no dupe found 
        /// </returns>
        public bool checkDupe(List<Pokemon> currTeam, Pokemon checkMon) {
            foreach (var i in currTeam) {
                if (i.checkDupe(checkMon.id)) {
                    return true;
                }
            }
            return false;
        }
    }
}