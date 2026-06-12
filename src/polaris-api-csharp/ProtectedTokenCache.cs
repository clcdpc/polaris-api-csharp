using Clc.Polaris.Api.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    internal static class ProtectedTokenCache
    {
        private const int MaxProtectedTokenCacheLockEntries = 1024;
        private const int TargetProtectedTokenCacheLockEntries = 512;

        private static readonly TimeSpan LockIdleLimit = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ExpirationSkew = TimeSpan.FromMinutes(1);
        private static readonly object LocksPruneLock = new();

        private static ConcurrentDictionary<string, ProtectedToken> Tokens { get; } = new ConcurrentDictionary<string, ProtectedToken>();
        private static ConcurrentDictionary<string, LockEntry> Locks { get; } = new ConcurrentDictionary<string, LockEntry>();

        internal static string? BuildKey(string hostname, string accessId, string accessKey, PolarisUser? staffOverrideAccount)
        {
            if (staffOverrideAccount == null ||
                string.IsNullOrWhiteSpace(hostname) ||
                string.IsNullOrWhiteSpace(accessId) ||
                string.IsNullOrWhiteSpace(accessKey) ||
                string.IsNullOrWhiteSpace(staffOverrideAccount.Domain) ||
                string.IsNullOrWhiteSpace(staffOverrideAccount.Username) ||
                string.IsNullOrWhiteSpace(staffOverrideAccount.Password))
            {
                return null;
            }

            return $"{hostname.Trim()}|{accessId.Trim()}|{staffOverrideAccount.Domain.Trim()}|{staffOverrideAccount.Username.Trim()}|{BuildCredentialFingerprint(accessKey, staffOverrideAccount.Password)}";
        }

        internal static bool IsUsable(ProtectedToken? token)
        {
            return !IsMissingOrExpired(token) && !string.IsNullOrWhiteSpace(token?.AccessToken) && !string.IsNullOrWhiteSpace(token.AccessSecret);
        }

        internal static bool TryGet(string? cacheKey, bool useCache, out ProtectedToken? protectedToken)
        {
            protectedToken = null;

            if (!useCache || string.IsNullOrWhiteSpace(cacheKey))
            {
                return false;
            }

            if (!Tokens.TryGetValue(cacheKey, out var cachedToken))
            {
                return false;
            }

            if (!IsUsable(cachedToken))
            {
                Tokens.TryRemove(cacheKey, out _);
                return false;
            }

            protectedToken = new ProtectedToken(cachedToken);
            return true;
        }

        internal static void Set(string? cacheKey, bool useCache, ProtectedToken token)
        {
            ArgumentNullException.ThrowIfNull(token);

            if (!useCache || string.IsNullOrWhiteSpace(cacheKey))
            {
                return;
            }

            Tokens[cacheKey] = new ProtectedToken(token);
        }

        internal static void Remove(string? cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                Tokens.TryRemove(cacheKey, out _);
            }
        }

        internal static async Task<ProtectedToken> GetOrCreateAsync(string? cacheKey, bool useCache, Func<CancellationToken, Task<ProtectedToken>> createTokenAsync, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(createTokenAsync);

            if (!CanUseCache(cacheKey, useCache))
            {
                return await createTokenAsync(cancellationToken).ConfigureAwait(false);
            }

            if (TryGet(cacheKey, useCache: true, out var cachedToken))
            {
                return cachedToken!;
            }

            using var cacheLockLease = await AcquireLockAsync(cacheKey!, cancellationToken).ConfigureAwait(false);

            if (TryGet(cacheKey, useCache: true, out cachedToken))
            {
                return cachedToken!;
            }

            var createdToken = await createTokenAsync(cancellationToken).ConfigureAwait(false);
            Set(cacheKey, useCache: true, createdToken);
            PruneExpired();

            return createdToken;
        }

        internal static int PruneExpired()
        {
            var removedCount = 0;

            foreach (var cachedToken in Tokens)
            {
                if (!IsUsable(cachedToken.Value) && Tokens.TryRemove(cachedToken.Key, out _))
                {
                    removedCount++;
                }
            }

            return removedCount;
        }

        private static async Task<IDisposable> AcquireLockAsync(string cacheKey, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var entry = Locks.GetOrAdd(cacheKey, _ => new LockEntry());

                if (!entry.TryAddLease())
                {
                    continue;
                }

                try
                {
                    await entry.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                    return new LockLease(entry);
                }
                catch
                {
                    entry.ReleaseLease();

                    if (Locks.Count > MaxProtectedTokenCacheLockEntries)
                    {
                        PruneLocks();
                    }

                    throw;
                }
            }
        }

        internal static void ClearForTesting()
        {
            Tokens.Clear();

            foreach (var cacheLock in Locks)
            {
                if (Locks.TryRemove(cacheLock.Key, out var removedEntry))
                {
                    removedEntry.Dispose();
                }
            }
        }

        internal static void AddForTesting(string cacheKey, ProtectedToken token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);
            ArgumentNullException.ThrowIfNull(token);

            Tokens[cacheKey] = new ProtectedToken(token);
        }

        internal static bool ContainsKey(string cacheKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);

            return Tokens.ContainsKey(cacheKey);
        }

        internal static int CountForTesting()
        {
            return Tokens.Count;
        }

        private static string BuildCredentialFingerprint(string accessKey, string staffPassword)
        {
            var credentialMaterial = $"access-key:{accessKey.Length}:{accessKey}|staff-password:{staffPassword.Length}:{staffPassword}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(credentialMaterial));
            return Convert.ToHexString(hash);
        }

        private static bool IsMissingOrExpired(ProtectedToken? token)
        {
            if (token == null || !token.ExpirationDate.HasValue)
            {
                return true;
            }

            var expirationUtc = NormalizeExpirationUtc(token.ExpirationDate.Value);

            return expirationUtc <= DateTime.UtcNow.Add(ExpirationSkew);
        }

        private static DateTime NormalizeExpirationUtc(DateTime expirationDate)
        {
            return expirationDate.Kind switch
            {
                DateTimeKind.Utc => expirationDate,
                DateTimeKind.Local => expirationDate.ToUniversalTime(),
                _ => DateTime.SpecifyKind(expirationDate, DateTimeKind.Utc)
            };
        }

        private static void PruneLocks()
        {
            if (Locks.Count <= MaxProtectedTokenCacheLockEntries)
            {
                return;
            }

            lock (LocksPruneLock)
            {
                if (Locks.Count <= MaxProtectedTokenCacheLockEntries)
                {
                    return;
                }

                var cutoffUtc = DateTime.UtcNow.Subtract(LockIdleLimit);

                PruneLocks(cutoffUtc, requireIdle: true);

                if (Locks.Count > MaxProtectedTokenCacheLockEntries)
                {
                    PruneLocks(cutoffUtc, requireIdle: false);
                }
            }
        }

        private static void PruneLocks(DateTime cutoffUtc, bool requireIdle)
        {
            foreach (var pair in Locks)
            {
                if (Locks.Count <= TargetProtectedTokenCacheLockEntries)
                {
                    return;
                }

                var entry = pair.Value;

                if (!entry.TryRetire(cutoffUtc, requireIdle))
                {
                    continue;
                }

                if (TryRemoveLockEntry(pair.Key, entry))
                {
                    entry.Dispose();
                }
                else
                {
                    entry.UndoRetire();
                }
            }
        }

        private static bool TryRemoveLockEntry(string cacheKey, LockEntry entry)
        {
            return ((ICollection<KeyValuePair<string, LockEntry>>)Locks)
                .Remove(new KeyValuePair<string, LockEntry>(cacheKey, entry));
        }

        private static bool CanUseCache(string? cacheKey, bool useCache)
        {
            return useCache && !string.IsNullOrWhiteSpace(cacheKey);
        }

        private sealed class LockLease : IDisposable
        {
            private LockEntry? _entry;

            public LockLease(LockEntry entry)
            {
                _entry = entry;
            }

            public void Dispose()
            {
                var entry = _entry;
                if (entry == null)
                {
                    return;
                }

                _entry = null;

                try
                {
                    entry.Semaphore.Release();
                }
                finally
                {
                    entry.ReleaseLease();

                    if (Locks.Count > MaxProtectedTokenCacheLockEntries)
                    {
                        PruneLocks();
                    }
                }
            }
        }

        private sealed class LockEntry : IDisposable
        {
            private readonly object _syncRoot = new();
            private int _leaseCount;
            private bool _retired;
            private DateTime _lastUsedUtc = DateTime.UtcNow;

            internal SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(1, 1);

            public bool TryAddLease()
            {
                lock (_syncRoot)
                {
                    if (_retired)
                    {
                        return false;
                    }

                    _leaseCount++;
                    _lastUsedUtc = DateTime.UtcNow;
                    return true;
                }
            }

            public void ReleaseLease()
            {
                lock (_syncRoot)
                {
                    if (_leaseCount > 0)
                    {
                        _leaseCount--;
                    }

                    _lastUsedUtc = DateTime.UtcNow;
                }
            }

            public bool TryRetire(DateTime cutoffUtc, bool requireIdle)
            {
                lock (_syncRoot)
                {
                    if (_retired || _leaseCount != 0)
                    {
                        return false;
                    }

                    if (requireIdle && _lastUsedUtc > cutoffUtc)
                    {
                        return false;
                    }

                    _retired = true;
                    return true;
                }
            }

            public void UndoRetire()
            {
                lock (_syncRoot)
                {
                    if (_retired && _leaseCount == 0)
                    {
                        _retired = false;
                    }
                }
            }

            public void Dispose()
            {
                Semaphore.Dispose();
            }
        }
    }
}
