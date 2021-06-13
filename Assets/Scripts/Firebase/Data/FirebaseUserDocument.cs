//    using Firebase.Data;
//
//    var firebaseUserDocument = FirebaseUserDocument.FromJson(jsonString);

namespace Firebase.Data
{
    using System;
    using System.Collections.Generic;
    
    using Newtonsoft.Json;

    public partial class FirebaseUserDocument
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fields")]
        public UserFields Fields { get; set; }

        [JsonProperty("createTime")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("updateTime")]
        public DateTimeOffset UpdateTime { get; set; }
    }

    public class UserFields
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

    public class Date
    {
        [JsonProperty("timestampValue")]
        public DateTimeOffset TimestampValue { get; set; }
    }

    public class Elo
    {
        [JsonProperty("integerValue")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IntegerValue { get; set; }
    }

    public class Email
    {
        [JsonProperty("stringValue")]
        public string StringValue { get; set; }
    }

    public class Items
    {
        [JsonProperty("arrayValue")]
        public ItemsArrayValue ArrayValue { get; set; }
    }

    public class ItemsArrayValue
    {
    }

    public class LikedNews
    {
        [JsonProperty("arrayValue")]
        public LikedNewsArrayValue ArrayValue { get; set; }
    }

    public class LikedNewsArrayValue
    {
        [JsonProperty("values")]
        public List<Email> Values { get; set; }
    }

    public partial class FirebaseUserDocument
    {
        public static FirebaseUserDocument FromJson(string json) => JsonConvert.DeserializeObject<FirebaseUserDocument>(json, Firebase.Data.Converter.Settings);
    }
}
