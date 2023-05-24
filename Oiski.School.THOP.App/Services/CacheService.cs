using System.Diagnostics;

namespace Oiski.School.THOP.App.Services
{
    public class CacheService
    {
        private readonly string _path;

        public CacheService()
        {
            _path = Path.Combine(FileSystem.Current.CacheDirectory, "latesthumidex.txt");
        }

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
