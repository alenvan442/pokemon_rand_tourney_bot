using System.Collections.Generic;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.model.utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace pokemon_rand.src.main.model.persistence 
{
    public class TournamentFileDAO : ObjectFileDAO<Tournament> {

        public TournamentFileDAO(string jsonString, JsonUtilities jsonUtilities) : base(jsonString, jsonUtilities) { }

    }
}
