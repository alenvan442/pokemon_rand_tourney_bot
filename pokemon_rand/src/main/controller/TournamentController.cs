using DSharpPlus.Entities;
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

        public bool host(DiscordMember member) {
            return this.tournamentDAO.newTourney(member.Id);
        }
    }
}