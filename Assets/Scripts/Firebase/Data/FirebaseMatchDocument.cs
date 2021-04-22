//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Firebase.Data;
//
//    var firebaseMatchDocument = FirebaseMatchDocument.FromJson(jsonString);

namespace Firebase.Data
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class FirebaseMatchDocument
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fields")]
        public FirebaseMatchDocumentFields Fields { get; set; }

        [JsonProperty("createTime")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("updateTime")]
        public DateTimeOffset UpdateTime { get; set; }

        public string GetId()
        {
            var t = this.Name.Split('/');
            return t[t.Length - 1];
        }
    }

    public partial class FirebaseMatchDocumentFields
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("winner")]
        public String Winner { get; set; }

        [JsonProperty("endDate")]
        public Date EndDate { get; set; }

        [JsonProperty("players")]
        public Players Players { get; set; }
    }

    public partial class Players
    {
        [JsonProperty("arrayValue")]
        public ArrayValue ArrayValue { get; set; }
    }

    public partial class ArrayValue
    {
        [JsonProperty("values")]
        public List<Value> Values { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("mapValue")]
        public MapValue MapValue { get; set; }
    }

    public partial class MapValue
    {
        [JsonProperty("fields")]
        public MapValueFields Fields { get; set; }
    }

    public partial class MapValueFields
    {
        [JsonProperty("assists")]
        public Number Assists { get; set; }

        [JsonProperty("uid")]
        public String Uid { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("team")]
        public Number Team { get; set; }

        [JsonProperty("deaths")]
        public Number Deaths { get; set; }

        [JsonProperty("kills")]
        public Number Kills { get; set; }
        
        [JsonProperty("points")]
        public Number Points { get; set; }
    }

    public partial class FirebaseMatchDocument
    {
        public static FirebaseMatchDocument FromJson(string json) => JsonConvert.DeserializeObject<FirebaseMatchDocument>(json, Firebase.Data.Converter.Settings);
    }
}
