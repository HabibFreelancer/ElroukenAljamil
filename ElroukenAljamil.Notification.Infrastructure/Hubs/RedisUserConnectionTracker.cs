using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ElroukenAljamil.Notification.Infrastructure.Hubs
{
    /// <summary>
    /// Implémentation Redis du tracker de connexions.
    /// Redis est nécessaire en multi-instances : si le Notification.Service est répliqué
    /// (load balancing), un Set Redis partagé garantit la cohérence des connexions.
    /// Chaque connexion est stockée dans un Redis Set par utilisateur.
    /// </summary>
    public class RedisUserConnectionTracker : IUserConnectionTracker
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisUserConnectionTracker> _logger;
        private const string KeyPrefix = "notification:connections:";

        public RedisUserConnectionTracker(
            IConnectionMultiplexer redis,
            ILogger<RedisUserConnectionTracker> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task AddConnectionAsync(Guid userId, string connectionId)
        {
            var db = _redis.GetDatabase();
            var key = $"{KeyPrefix}{userId}";

            await db.SetAddAsync(key, connectionId);
            // Expiration de sécurité (24h) au cas où un disconnect serait manqué
            await db.KeyExpireAsync(key, TimeSpan.FromHours(24));

            _logger.LogDebug("Connexion {ConnId} ajoutée pour {UserId}.", connectionId, userId);
        }

        public async Task RemoveConnectionAsync(Guid userId, string connectionId)
        {
            var db = _redis.GetDatabase();
            var key = $"{KeyPrefix}{userId}";

            await db.SetRemoveAsync(key, connectionId);

            // Supprimer la clé si plus aucune connexion
            var remaining = await db.SetLengthAsync(key);
            if (remaining == 0)
                await db.KeyDeleteAsync(key);

            _logger.LogDebug("Connexion {ConnId} retirée pour {UserId}.", connectionId, userId);
        }

        public async Task<bool> IsOnlineAsync(Guid userId)
        {
            var db = _redis.GetDatabase();
            var key = $"{KeyPrefix}{userId}";
            return await db.SetLengthAsync(key) > 0;
        }

        public async Task<IReadOnlyList<string>> GetConnectionsAsync(Guid userId)
        {
            var db = _redis.GetDatabase();
            var key = $"{KeyPrefix}{userId}";
            var members = await db.SetMembersAsync(key);
            return members.Select(m => m.ToString()).ToList();
        }

        public async Task<int> GetOnlineCountAsync()
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{KeyPrefix}*");
            return keys.Count();
        }
    }

}
