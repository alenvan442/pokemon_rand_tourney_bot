using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;

namespace pokemon_rand.src.main.controller
{
    public class TournamentController : ObjectController<Tournament>
    {

        TournamentFileDAO tournamentDAO;

        public TournamentController(TournamentFileDAO tournamentDAO) : base(tournamentDAO)
        {
            this.tournamentDAO = tournamentDAO;
        }

    }
}