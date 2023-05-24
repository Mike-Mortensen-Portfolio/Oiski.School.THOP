using System.Text.Json;

namespace Oiski.School.THOP.App.Services
{
    public static class Extensions
    {
        public static string ToJson<TObject>(this TObject obj)
        {
            var output = "NULL";
            if (obj != null)
                output = JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            return output;
        }

        public static TObject FromJson<TObject>(this TObject obj, string json)
        {
            return JsonSerializer.Deserialize<TObject>(json);
        }
    }
}
