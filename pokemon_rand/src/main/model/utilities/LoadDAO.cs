using System.IO;
using DSharpPlus.Entities;
using pokemon_rand.src.main.controller;
using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;
using pokemon_rand.src.main.view.discord.commands;

namespace pokemon_rand.src.main.model.utilities
{
    public static class LoadDAO
    {
        static PlayersFileDAO? playersFileDAO;
        static PlayerController? playerController;
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

            playerController = new PlayerController(playersFileDAO);

            //exhibitionController = new ObjectController<Exhibition>(exhibitionFileDAO);

            CommandsHelper.setup(playerController);
            
        }

        public static void loadTournaments() {
            var files = Directory.GetFiles(StaticUtil.tournamentFolder);
        }
        
    }
}