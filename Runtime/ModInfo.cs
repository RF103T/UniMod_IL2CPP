using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Katas.UniMod
{
    /// <summary>
    /// Structure for the mod's info.json file
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct ModInfo
    {
        [JsonRequired]
        public ModTargetInfo Target;
        [JsonRequired]
        public string ModId;
        [JsonRequired]
        public string ModVersion;
        [JsonRequired][JsonConverter(typeof(StringEnumConverter))]
        public ModType Type;
        public string DisplayName;
        public string Description;
        public Dictionary<string, string> Dependencies;
    }

    public struct ModTargetInfo
    {
        [JsonRequired]
        public string UniModVersion;
        public string TargetId;
        public string TargetVersion;
        public string Platform;
    }

    public enum ModType
    {
        ContentAndAssemblies = 0,
        Content = 1,
        Assemblies = 2
    }
}
