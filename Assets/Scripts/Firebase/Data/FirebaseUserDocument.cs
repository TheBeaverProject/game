// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Firebase.Data;
//
//    var firebaseUserDocument = FirebaseUserDocument.FromJson(jsonString);

namespace Firebase.Data
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class FirebaseUserDocument
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fields")]
        public Fields Fields { get; set; }

        [JsonProperty("createTime")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("updateTime")]
        public DateTimeOffset UpdateTime { get; set; }
    }

    public partial class Fields
    {
        [JsonProperty("level")]
        public Elo Level { get; set; }

        [JsonProperty("matchHistory")]
        public Items MatchHistory { get; set; }

        [JsonProperty("elo")]
        public Elo Elo { get; set; }

        [JsonProperty("registerDate")]
        public Date RegisterDate { get; set; }

        [JsonProperty("items")]
        public Items Items { get; set; }

        [JsonProperty("username")]
        public Email Username { get; set; }

        [JsonProperty("likedNews")]
        public LikedNews LikedNews { get; set; }

        [JsonProperty("email")]
        public Email Email { get; set; }

        [JsonProperty("birthdate")]
        public Date Birthdate { get; set; }

        [JsonProperty("status")]
        public Elo Status { get; set; }
    }

    public partial class Date
    {
        [JsonProperty("timestampValue")]
        public DateTimeOffset TimestampValue { get; set; }
    }

    public partial class Elo
    {
        [JsonProperty("integerValue")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IntegerValue { get; set; }
    }

    public partial class Email
    {
        [JsonProperty("stringValue")]
        public string StringValue { get; set; }
    }

    public partial class Items
    {
        [JsonProperty("arrayValue")]
        public ItemsArrayValue ArrayValue { get; set; }
    }

    public partial class ItemsArrayValue
    {
    }

    public partial class LikedNews
    {
        [JsonProperty("arrayValue")]
        public LikedNewsArrayValue ArrayValue { get; set; }
    }

    public partial class LikedNewsArrayValue
    {
        [JsonProperty("values")]
        public List<Email> Values { get; set; }
    }

    public partial class FirebaseUserDocument
    {
        public static FirebaseUserDocument FromJson(string json) => JsonConvert.DeserializeObject<FirebaseUserDocument>(json, Firebase.Data.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this FirebaseUserDocument self) => JsonConvert.SerializeObject(self, Firebase.Data.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
