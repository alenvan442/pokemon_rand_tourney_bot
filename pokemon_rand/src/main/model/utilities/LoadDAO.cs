using System.IO;
using System.Text.RegularExpressions;
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

            tournamentController = new TournamentController(tournamentFileDAO, playersFileDAO);
            playerController = new PlayerController(playersFileDAO, pokemonFileDAO, tournamentFileDAO, tournamentController);

            //exhibitionController = new ObjectController<Exhibition>(exhibitionFileDAO);

            CommandsHelper.setup(playerController, tournamentController);

            // set up pokemom database
            /*
            Dictionary<ulong, List<ulong>> evoLines = new Dictionary<ulong, List<ulong>>();
            Dictionary<ulong, string> names = new Dictionary<ulong, string>();

            using(var streamReader = File.OpenText("pokemon_rand/data/pokemon_names")) {
                var lines = streamReader.ReadToEnd().Split("\n");
                foreach (var i in lines) {
                    string line = i.Trim();
                    string[] parts = line.Split(":");
                    names.Add((ulong)Int32.Parse(parts[0]), parts[1]);
                }
            }

            using (var streamReader = File.OpenText("pokemon_rand/data/Pokemon EvoLines.rtf")) {
                var lines = streamReader.ReadToEnd().Split("\n");
                foreach (var i in lines) {
                    string line = Regex.Replace(i, "[()]", "").Trim();
                    string[] ids = line.Split(",");
                    List<ulong> evos = new List<ulong>();
                    foreach (string id in ids) {
                        evos.Add((ulong)Int32.Parse(id));
                    }
                    foreach (ulong id in evos) {
                        if (!evoLines.TryGetValue(id, out _)) {
                            evoLines.Add(id, evos);
                        }
                    }
                }
            }

            for (int i = 1; i <= 1010; i++) {
                string imageId = i.ToString();

                if (i < 10) {
                    imageId = "00" + i;
                } else if (i >= 10 && i < 100) {
                    imageId = "0" + i;
                }

                string imagePath = "pokemon_rand/data/Pokemon Sprites/" + imageId + ".png";
                List<ulong> evoline = evoLines.TryGetValue((ulong)i, out _) ? evoLines[(ulong)i] : new List<ulong>();
                Pokemon pokemon = new Pokemon((ulong)i, names[(ulong)i], imagePath, evoline);
                pokemonFileDAO.addObject(pokemon, pokemon.id);
            }  
            */
        }    
    }
}