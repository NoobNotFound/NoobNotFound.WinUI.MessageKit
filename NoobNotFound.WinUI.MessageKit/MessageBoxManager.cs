using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoobNotFound.WinUI.MessageKit;

/// <summary>
/// Manages MessageBox queue to prevent multiple dialogs from interfering
/// </summary>
public static class MessageBoxManager
{
    private static readonly ConcurrentQueue<Func<Task<MessageBoxResponse>>> _messageQueue = new();
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static readonly Dictionary<string, MessageBoxResponse> _responseCache = new();

    /// <summary>
    /// Shows a MessageBox with queueing support
    /// </summary>
    public static async Task<MessageBoxResponse> ShowQueuedAsync(MessageBoxOptions options)
    {
        // Check cache first
        var cacheKey = GenerateCacheKey(options);
        if (_responseCache.TryGetValue(cacheKey, out var cachedResponse))
        {
            return cachedResponse;
        }

        await _semaphore.WaitAsync();

        try
        {
            var response = await MessageBox.ShowImmediateAsync(options);

            // Cache the response if appropriate
            if (ShouldCacheResponse(options))
            {
                _responseCache[cacheKey] = response;
            }

            return response;
        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static string GenerateCacheKey(MessageBoxOptions options) =>
        $"{options.Title}|{options.Message}|{options.Buttons}";

    private static bool ShouldCacheResponse(MessageBoxOptions options) =>
        options.Metadata.ContainsKey("Cache") && (bool)options.Metadata["Cache"];

    public static void ClearCache() => _responseCache.Clear();
}