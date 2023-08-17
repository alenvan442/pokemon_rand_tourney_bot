using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;
using pokemon_rand_tourney_bot.pokemon_rand.src.main.model.persistence;

namespace pokemon_rand.src.main.model.utilities
{
    public static class IdentificationSearch
    {

        static PokemonFileDAO? pokemonDAO;
        static TournamentFileDAO? tournamentDAO;

        /// <summary>
        /// initializes this static class
        /// </summary>
        public static void init(PokemonFileDAO _pokemonDAO, TournamentFileDAO _tournamentDAO) {
            pokemonDAO = _pokemonDAO;
            tournamentDAO = _tournamentDAO;
        }

        public static Pokemon pokemonSearch(ulong id) {
            return pokemonDAO.getObject(id);
        }

        public static Tournament tournamentSearch(ulong id) {
            return tournamentDAO.getObject(id);
        }

        /*
        public static Exhibition exhibitionSearch(uint id) {
            return exhibitionsFileDAO.getObject(id);
        }
        */

    }
}