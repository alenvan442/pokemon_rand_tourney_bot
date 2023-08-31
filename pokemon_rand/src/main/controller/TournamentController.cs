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
        /// gets whether or not the caller is the host
        /// </summary>
        /// <param name="member">the caller</param>
        /// <returns>
        ///     true: the caller is the host
        ///     false: the caller is not the host 
        /// </returns>
        public bool isHost(DiscordMember target) {
            Player caller = this.playerDAO.getObject(target.Id);
            Tournament currentTourney = this.getObject(caller.currentTournamentId);

            if (currentTourney is null) {
                return false;
            }

            return currentTourney.hostId == caller.id; 
        }

        // used for force join
        public bool isHost(DiscordMember target, ulong tourneyId) {
            Player caller = this.playerDAO.getObject(target.Id);
            Tournament currentTournament = this.getObject(tourneyId);
            return currentTournament is null ? false : caller.id == currentTournament.hostId;
        }

        // used for force reroll
        public bool isHost(DiscordMember target, DiscordMember player) {
            Player caller = this.playerDAO.getObject(target.Id);
            Player reference = this.playerDAO.getObject(player.Id);
            Tournament currentTourney = this.getObject(caller.currentTournamentId);

            return ((currentTourney is null) ? false : currentTourney.hostId == caller.id) ||
                    reference.tournaments.Contains(currentTourney.hostId);
        }

        public bool isHost(DiscordMember target, DiscordMember playerOne, DiscordMember playerTwo) {
            Player caller = this.playerDAO.getObject(target.Id);
            Player one = this.playerDAO.getObject(playerOne.Id);
            Player two = this.playerDAO.getObject(playerTwo.Id);
            Tournament currentTourney = this.getObject(caller.currentTournamentId);
            
            bool oneTourney = one.tournaments.Contains(caller.id);
            bool twoTourney = two.tournaments.Contains(caller.id);
            return ((currentTourney is null) ? false : currentTourney.hostId == caller.id) ||
                    (oneTourney && twoTourney); // are you the host of the tournament you're currently participating it
                                                // OR are you the host of a tournament both participants are in.
        }

        public bool join(ulong target, ulong tourneyId, bool ovRide = false) {
            return this.tournamentDAO.join(tourneyId, target, ovRide);
        }

        public bool leave(ulong target, ulong tourneyId, bool add = true) {
            return this.tournamentDAO.leave(tourneyId, target, add);
        }

        /// <summary>
        /// sets the result of a match, only the host can do this
        /// the match is read with player one being the main player, example:
        /// playerOne (0, 1, 2) playerTwo, where (0, 1, 2) are the possible outcomes of the match.
        ///     0: lose
        ///     1: win
        ///     2: tie
        /// so playerOne (lost against, won against, tied against) playerTwo
        /// </summary> 
        /// <param name="tourneyId">the id of the tournament the match was in</param>
        /// <param name="playerOne">the first player</param>
        /// <param name="playerTwo">the second player</param>
        /// <param name="score">the result of the match</param>
        /// <returns>
        ///     true: the score was set successfully
        ///     false: one or more of the players is not in the tournament OR the two have already fought eachother 
        /// </returns>
        public bool setScore(DiscordMember caller, ulong playerOne, ulong playerTwo, int score) {
            ulong tourneyId = caller.Id;
            bool result;

            result = this.playerDAO.setScore(tourneyId, playerOne, playerTwo, score);

            if (!result) {
                return false;
            }

            result = this.tournamentDAO.setScore(tourneyId, playerOne, playerTwo, score);

            if (!result) {
                this.playerDAO.deleteScore(tourneyId, playerOne, playerTwo);
                return false;
            }

            return true;
        }

        /// <summary>
        /// gets the leaderboard of the current tournament
        /// </summary>
        /// <param name="member">the member that invoked this command</param>
        /// <returns>
        ///     returns a dictionary where each player is the key with their value being their total scores
        ///     null if the user is currently not in a tournament
        /// </returns>
        public List<Tuple<Player, int, int, int>> getLeaderboard(DiscordMember member) {
            List<Tuple<Player, int, int, int>> leaderboard = new List<Tuple<Player, int, int, int>>();
            Dictionary<Player, List<int>> records = new Dictionary<Player, List<int>>();

            Player player = this.playerDAO.getObject(member.Id);

            if (player.currentTournamentId == 0) {return null;}

            Tournament currTourney = this.getObject(player.currentTournamentId);
            List<ulong> playerIds = currTourney.players;

            foreach (var i in playerIds) {
                Player currPlayer = this.playerDAO.getObject(i);
                List<int> scores = currPlayer.getScore(player.currentTournamentId);
                scores.Add(getUnmatched(currTourney, scores));
                records.Add(currPlayer, scores);
            }

            List<ulong> sorted = this.sortLeaderboard(records);

            // fill the full leaderboard
            foreach (ulong i in sorted) {
                Player target = this.playerDAO.getObject(i);
                List<int> record = records[target];
                leaderboard.Add(new Tuple<Player, int, int, int>(target, record[0], record[1], record[2]));
            }

            return leaderboard;

        }

        /// <summary>
        /// gets a target player's score
        /// </summary>
        /// <param name="member">the player who incoked the command</param>
        /// <param name="target">the player that the caller wishes to view their score</param>
        /// <returns>
        ///     a list of ints representing the player's standing
        ///     (wins, losts, ties, unmatched) 
        ///     
        ///     null is the player is currently not in a tournament 
        /// </returns>
        public List<int> getPlayerScore(DiscordMember member, DiscordMember target = null) { // test later to see what gets passed in if we use a mention as an argument
            if (target == null) {
                target = member;
            }
            Player targetPlayer = this.playerDAO.getObject(target.Id);

            if (targetPlayer.currentTournamentId == 0) {return null;}

            List<int> scores = targetPlayer.getScore(targetPlayer.currentTournamentId);
            scores.Add(getUnmatched(this.getObject(targetPlayer.currentTournamentId), scores));
            return scores;
        }

        /// <summary>
        /// get the list of all matches of the tournament
        /// </summary>
        /// <param name="member">the caller</param>
        /// <returns>
        ///     a list of matches in the format: (playerOne, result, playerTwo),
        ///     null if the player is not currently in a tournament.
        /// </returns>
        public List<string> getHistory(DiscordMember member, int pageNumber = 1) {
            Player caller = this.playerDAO.getObject(member.Id);
            if (caller.currentTournamentId == 0) {return null;}
            List<List<ulong>> history = this.getObject(caller.currentTournamentId).history;
            List<string> result = new List<string>();
 
            for (int i = history.Count - 1; i >= 0; i--) {
                List<ulong> curr = history[i];
                Player one = this.playerDAO.getObject(curr[0]);
                Player two = this.playerDAO.getObject(curr[2]);
                string score = "";

                if (curr[1] == 0) { score = " Lost To "; }
                else if (curr[1] == 1) { score = " Won Against "; }
                else if (curr[1] == 2) { score = " Tied against "; }

                result.Add(one.name + score + two.name);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<string> getTournaments(DiscordMember member, int pageNumber = 1) {
            List<Tournament> tournaments = this.getObjects().ToList();
            
            if (tournaments is null || tournaments.Count == 0) {
                return null;
            }

            List<string> result = new List<string>();

            for (int i = 0; i < tournaments.Count; i++) {
                result.Add(this.playerDAO.getObject(tournaments[i].hostId).name + "'s Tournament");
            }

            return result;
        }

        /// <summary>
        /// private function that determines how many battles the player has
        /// </summary>
        /// <param name="tourney">the current tournament</param>
        /// <param name="scores">the current scores of the player (total battles fought so far)</param>
        /// <returns>
        ///     an int that holds how many battles are left
        /// </returns>
        private int getUnmatched(Tournament tourney, List<int> scores) {
            // scores will ALWAYS have 3 integers in it's list where:
            //      [0]: number wins
            //      [1]: number losses
            //      [2]: number ties
            int totalPlayers = tourney.players.Count() - 1; //subtract one so it does not include yourself
            int totalMatched = scores[0] + scores[1] + scores[2];
            return totalPlayers - totalMatched;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scores"></param>
        /// <returns></returns>
        private List<ulong> sortLeaderboard(Dictionary<Player, List<int>> scores) {
            List<ulong> sorted = new List<ulong>();
            List<Tuple<ulong, int>> list = new List<Tuple<ulong, int>>();
            
            // convert to sortable list
            foreach (Player i in scores.Keys) {
                int totalScore = 0;
                List<int> record = scores[i];
                totalScore += record[0] * 3;
                totalScore += record[2];
                list.Add(new Tuple<ulong, int>(i.id, totalScore));
            }

            // sort the list
            list = this.sortLeaderboardHelper(list);

            foreach (Tuple<ulong, int> i in list) {
                sorted.Add(i.Item1);
            } 

            return sorted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Tuple<ulong, int>> sortLeaderboardHelper(List<Tuple<ulong, int>> list) {
            if (list.Count() < 3) {
                return list;
            }

            List<Tuple<ulong, int>> first = new List<Tuple<ulong, int>>();
            List<Tuple<ulong, int>> second = new List<Tuple<ulong, int>>();
            Tuple<ulong, int> middle = list[list.Count / 2];

            foreach (Tuple<ulong, int> i in list) {
                if (middle.Item1 == i.Item1) {
                    continue;
                }
                if (i.Item2 > middle.Item2 || i.Item2 == middle.Item2) {
                    first.Add(i);
                } else if (i.Item2 < middle.Item2) {
                    second.Add(i);
                }
            }

            first = this.sortLeaderboardHelper(first);
            second = this.sortLeaderboardHelper(second);

            first.Add(middle);
            first.AddRange(second);

            return first;
        }

    }
}