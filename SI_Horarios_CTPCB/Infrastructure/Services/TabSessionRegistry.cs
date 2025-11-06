using System.Collections.Concurrent;

namespace SI_Horarios_CTPCB.Infrastructure.Services
{
    public class TabSessionRegistry
    {
        // userId -> activeStamp
        private readonly ConcurrentDictionary<string, string> _active = new();

        public string RegisterNewStamp(string userId)
        {
            var stamp = Guid.NewGuid().ToString("N");
            _active[userId] = stamp;
            return stamp;
        }

        public string? GetActiveStamp(string userId)
        {
            return _active.TryGetValue(userId, out var stamp) ? stamp : null;
        }

        public bool IsCurrent(string userId, string myStamp)
        {
            return GetActiveStamp(userId) == myStamp;
        }
    }
}
