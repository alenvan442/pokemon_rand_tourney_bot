using System.IO;
using DSharpPlus.Entities;
using pokemon_rand.src.main.controller;
using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.view.discord.commands;
using pokemon_rand_tourney_bot.pokemon_rand.src.main.model.persistence;

namespace pokemon_rand.src.main.model.utilities
{
    public static class LoadDAO
    {
        private static PlayersFileDAO? playersFileDAO;
        private static PokemonFileDAO? pokemonFileDAO;
        private static TournamentFileDAO? tournamentFileDAO;

        private static PlayerController? playerController;
        private static TournamentController? tournamentController;
        //static ObjectFileDAO<Exhibition>? exhibitionFileDAO;
        //static ObjectController<Exhibition>? exhibitionController;

        /// <summary>
        /// Loads the FileDAOs
        /// </summary>
        public static void load() {
            JsonUtilities json = new JsonUtilities();

            //exhibitionFileDAO = new ObjectFileDAO<Exhibition>(StaticUtil.exhibitionJson, json);
            
            //IdentificationSearch.init(employeeFileDAO, exhibitionFileDAO, null);

            playersFileDAO = new PlayersFileDAO(StaticUtil.playersJson, json);
            pokemonFileDAO = new PokemonFileDAO(StaticUtil.pokemonJson, json);
            tournamentFileDAO = new TournamentFileDAO(StaticUtil.tournamentJson, json);

            playerController = new PlayerController(playersFileDAO, pokemonFileDAO, tournamentFileDAO);
            tournamentController = new TournamentController(tournamentFileDAO, playersFileDAO);

            //exhibitionController = new ObjectController<Exhibition>(exhibitionFileDAO);

            CommandsHelper.setup(playerController);
            
        }        
    }
}