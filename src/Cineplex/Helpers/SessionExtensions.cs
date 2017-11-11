using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Helpers
{
    // From MSDN
    // Serialise session with JSON.NET
    // Slightly modified to allow self-referencing exceptions,
    // esp for serialising bi-directional relationships in EF Core
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, Formatting.Indented,
                    // ignore self referencing
                    new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }
            ));

        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }
    }
}
