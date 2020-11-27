using System;
using System.Runtime.Caching;

namespace JssSnippet.Services
{
    static class SnippetCacheService
    {
        const int _lifeTime = 20;

        public static T Get<T>(string key) where T : class
        {
            return MemoryCache.Default.Get(key) as T;
        }

        public static void Upset<T>(T value, string key)
        {
            if (MemoryCache.Default.Get(key) == null)
                MemoryCache.Default.Add(key, value, DateTime.Now.AddMinutes(_lifeTime));
            else
                MemoryCache.Default.Set(key, value, DateTime.Now.AddMinutes(_lifeTime));
        }

        public static void Delete(string key)
        {
            if (MemoryCache.Default.Contains(key))
                MemoryCache.Default.Remove(key);
        }
    }
}