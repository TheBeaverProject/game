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
        public Number Level { get; set; }

        [JsonProperty("matchHistory")]
        public FirestoreStringList MatchHistory { get; set; }

        [JsonProperty("elo")]
        public Number Elo { get; set; }

        [JsonProperty("registerDate")]
        public Date RegisterDate { get; set; }

        [JsonProperty("items")]
        public FirestoreStringList Items { get; set; }

        [JsonProperty("username")]
        public String Username { get; set; }

        [JsonProperty("likedNews")]
        public FirestoreStringList LikedNews { get; set; }

        [JsonProperty("email")]
        public String Email { get; set; }

        [JsonProperty("birthdate")]
        public Date Birthdate { get; set; }

        [JsonProperty("status")]
        public Number Status { get; set; }
    }

    public partial class FirebaseUserDocument
    {
        public static FirebaseUserDocument FromJson(string json) => JsonConvert.DeserializeObject<FirebaseUserDocument>(json, Firebase.Data.Converter.Settings);

        public FirebaseUserDocument()
        {
            Name = "";
            
            Fields = new UserFields
            {
                Birthdate = new Date(),
                Elo = new Number(),
                Email = new String(),
                Items = new FirestoreStringList(),
                Level = new Number(),
                LikedNews = new FirestoreStringList(),
                MatchHistory = new FirestoreStringList(),
                RegisterDate = new Date(),
                Status = new Number(),
                Username = new String()
            };
            
            CreateTime = DateTimeOffset.Now;
            UpdateTime = DateTimeOffset.Now;
        }
    }
}
