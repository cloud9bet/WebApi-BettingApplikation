using System.Collections.Concurrent;

namespace BettingApi.Services
{
    public interface ICrashGameStateService
    {
        bool CreateGame(int userAccountId, int betAmount, double crashPoint);
        bool TryGetGame(int userAccountId, out ActiveCrashGameState? game);
        bool RemoveGame(int userAccountId);
    }

    public class ActiveCrashGameState
    {
        public int BetAmount { get; set; }
        public double CrashPoint { get; set; }
        public DateTime StartTime { get; set; }
    }

    public class CrashGameStateService : ICrashGameStateService
    {
        // Thread-safe dictionary for tracking active games
        private readonly ConcurrentDictionary<int, ActiveCrashGameState> _activeGames = new();

        public bool CreateGame(int userAccountId, int betAmount, double crashPoint)
        {
            var game = new ActiveCrashGameState
            {
                BetAmount = betAmount,
                CrashPoint = crashPoint,
                StartTime = DateTime.UtcNow
            };

            return _activeGames.TryAdd(userAccountId, game);
        }

        public bool TryGetGame(int userAccountId, out ActiveCrashGameState? game)
        {
            return _activeGames.TryGetValue(userAccountId, out game);
        }

        public bool RemoveGame(int userAccountId)
        {
            return _activeGames.TryRemove(userAccountId, out _);
        }
    }
}
