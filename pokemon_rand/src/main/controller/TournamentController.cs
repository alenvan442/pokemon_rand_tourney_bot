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

        /// <summary>
        /// create a new tournament with the playe rinvoking this the host
        /// </summary>
        /// <param name="member">the new host</param>
        /// <returns>
        ///     true: tournament created successfully
        ///     false: user is already a host for a tournament 
        /// </returns>
        public bool host(DiscordMember member) {
            return this.tournamentDAO.newTourney(member.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tourneyId"></param>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        /// <param name="score"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tourneyId"></param>
        /// <returns></returns>
        public Dictionary<Player, List<int>> getLeaderboard(ulong tourneyId) {
            Dictionary<Player, List<int>> leaderboard = new Dictionary<Player, List<int>>();

            Tournament currTourney = this.getObject(tourneyId);
            List<ulong> playerIds = currTourney.players;

            foreach (var i in playerIds) {
                Player currPlayer = this.playerDAO.getObject(i);
                List<int> scores = currPlayer.getScore(tourneyId);
                scores.Add(getUnmatched(currTourney, scores));
                leaderboard.Add(currPlayer, scores);
            }

            return leaderboard;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="tourneyId"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<int> getPlayerScore(DiscordMember member, ulong tourneyId, DiscordMember target = null) { // test later to see what gets passed in if we use a mention as an argument
            if (target == null) {
                target = member;
            }
            Player targetPlayer = this.playerDAO.getObject(target.Id);
            return targetPlayer.getScore(tourneyId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tourneyId"></param>
        /// <returns></returns>
        public List<List<ulong>> getHistory(ulong tourneyId) {
            return this.getObject(tourneyId).history;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tourney"></param>
        /// <param name="scores"></param>
        /// <returns></returns>
        private int getUnmatched(Tournament tourney, List<int> scores) {
            // scores will ALWAYS have 3 integers in it's list where:
            //      [0]: number wins
            //      [1]: number losses
            //      [2]: number ties
            int totalPlayers = tourney.players.Count();
            int totalMatched = scores[0] + scores[1] + scores[2];
            return totalPlayers - totalMatched;
        }

    }
}