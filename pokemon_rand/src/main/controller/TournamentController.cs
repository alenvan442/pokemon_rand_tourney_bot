using DSharpPlus.Entities;
using pokemon_rand.src.main.model.persistence;
using pokemon_rand.src.main.model.structures;

namespace pokemon_rand.src.main.controller
{
    public class TournamentController : ObjectController<Tournament>
    {

        TournamentFileDAO tournamentDAO;
        PlayersFileDAO playerDAO;

        public TournamentController(TournamentFileDAO tournamentDAO, PlayersFileDAO playerDAO) : base(tournamentDAO)
        {
            this.tournamentDAO = tournamentDAO;
            this.playerDAO = playerDAO;
        }

        public bool host(DiscordMember member) {
            return this.tournamentDAO.newTourney(member.Id);
        }

        public bool setScore(ulong tourneyId, ulong playerOne, ulong playerTwo, int score) {
            bool result;

            result = this.tournamentDAO.setScore(tourneyId, playerOne, playerTwo, score);

            if (!result) {
                this.tournamentDAO.deleteScore(tourneyId, playerOne, playerTwo);
                return false;
            }

            result = this.playerDAO.setScore(tourneyId, playerOne, playerTwo, score);

            if (!result) {
                this.tournamentDAO.deleteScore(tourneyId, playerOne, playerTwo);
                this.playerDAO.deleteScore(tourneyId, playerOne, playerTwo);
                return false;
            }

            return true;
        }

    }
}