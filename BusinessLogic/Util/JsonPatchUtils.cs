using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;

namespace BusinessLogic.Util
{
    public static class JsonPatchUtils
    {
        public static JsonPatchDocument ToJsonPatchDocument(this object changes)
        {
            var patch = new JsonPatchDocument();
            var properties = (changes as IEnumerable<KeyValuePair<string, JToken>>);
            foreach (var property in properties)
                patch.Replace($"/{property.Key}", property.Value);
            return patch;
        }
    }
}
