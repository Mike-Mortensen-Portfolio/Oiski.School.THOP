using Oiski.School.THOP.Services;
using System.Diagnostics;

namespace Oiski.School.THOP.App.Services
{
    /// <summary>
    /// Represents a simple caching service for a <strong>.NET MAUI</strong> application
    /// </summary>
    public class CacheService
    {
        private readonly string _path;

        /// <summary>
        /// Instantiates a new instance of type <see cref="CacheService"/>
        /// </summary>
        public CacheService()
        {
            _path = Path.Combine(FileSystem.Current.CacheDirectory, "latesthumidex.txt");
        }

        /// <summary>
        /// Write the current state of <paramref name="obj"/> into the cache (<i>This will override any previous state data in the cache</i>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task CacheState<T>(T obj)
        {
            try
            {
                await File.WriteAllTextAsync(_path, obj.ToJson());
            }
            catch (Exception e)
            {

                Debug.WriteLine($"Cannot write cache: {e.Message}");
            }
        }

        /// <summary>
        /// Read the cached state as a type of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="Task"/> that represents the <see langword="asynchronous"/> operation</returns>
        public async Task<T> GetCache<T>() where T : new()
        {
            string cacheJson = null;
            try
            {
                cacheJson = await File.ReadAllTextAsync(_path);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"No cache available: {e.Message}");
            }


            return new T().FromJson(cacheJson);
        }
    }
}
