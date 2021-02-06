//    using Firebase.Data;
//
//    var firebaseAuthResponse = FirebaseAuthResponse.FromJson(jsonString);

namespace Firebase.Data
{
    using Newtonsoft.Json;

    public partial class FirebaseAuthResponse
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("localId")]
        public string LocalId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("idToken")]
        public string IdToken { get; set; }

        [JsonProperty("registered")]
        public bool Registered { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("expiresIn")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ExpiresIn { get; set; }
    }

    public partial class FirebaseAuthResponse
    {
        public static FirebaseAuthResponse FromJson(string json) => JsonConvert.DeserializeObject<FirebaseAuthResponse>(json, Firebase.Data.Converter.Settings);
    }
}
