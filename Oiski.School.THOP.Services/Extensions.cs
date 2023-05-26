using System.Text.Json;

namespace Oiski.School.THOP.Services
{
    public static class Extensions
    {
        /// <summary>
        /// Dump <paramref name="obj"/> to the <see cref="Console"/> as a <strong>JSON</strong> <see langword="object"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TObject DumpToConsole<TObject>(this TObject obj)
        {
            var output = "NULL";
            if (obj != null)
                output = JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            Console.WriteLine($"[{obj?.GetType().Name}]:\r\n{output}");

            return obj;
        }

        /// <summary>
        /// Parse an <see langword="object"/> of type <typeparamref name="TObject"/> to a <strong>JSON</strong> <see langword="string"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <returns>A new <see langword="string"/> that represents <paramref name="obj"/> in <strong>JSON</strong> if I could be parsed; otherwise, if not, "NULL as a <see langword="string"/>"</returns>
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

        /// <summary>
        /// Parse <paramref name="json"/> to an <see langword="object"/> of type <typeparamref name="TObject"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="json"></param>
        /// <returns>A new instance of type <typeparamref name="TObject"/> or <see langword="null"/> if <paramref name="json"/> coudn't be parsed as <typeparamref name="TObject"/></returns>
        public static TObject FromJson<TObject>(this TObject obj, string json)
        {
            return JsonSerializer.Deserialize<TObject>(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToQueryString<T>(this T obj, bool ignoreNull = false) where T : new()
        {
            string queryString = string.Empty;
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var propertyName = properties[i].Name;
                var propertyValue = properties[i].GetValue(obj);

                if (ignoreNull && propertyValue == null)
                    continue;

                queryString += $"{propertyName}={propertyValue}{((i + 1 < properties.Length) ? ("&") : (string.Empty))}";
            }

            return queryString;
        }
    }
}