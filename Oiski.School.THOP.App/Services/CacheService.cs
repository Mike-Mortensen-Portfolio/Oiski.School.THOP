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
            await File.WriteAllTextAsync(_path, obj.ToJson());
        }

        public async Task<T> GetCache<T>() where T : new()
        {
            string cacheJson = await File.ReadAllTextAsync(_path);

            return new T().FromJson(cacheJson);
        }
    }
}
