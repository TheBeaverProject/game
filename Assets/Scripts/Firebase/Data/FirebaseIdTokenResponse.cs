//    using Firebase.Data;
//
//    var firebaseIdTokenResponse = FirebaseIdTokenResponse.FromJson(jsonString);

namespace Firebase.Data
{
    using Newtonsoft.Json;

    public partial class FirebaseIdTokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ExpiresIn { get; set; }

        [JsonProperty("token_type")] public string TokenType { get; set; }

        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }

        [JsonProperty("id_token")] public string IdToken { get; set; }

        [JsonProperty("user_id")] public string UserId { get; set; }

        [JsonProperty("project_id")] public string ProjectId { get; set; }
    }

    public partial class FirebaseIdTokenResponse
    {
        public static FirebaseIdTokenResponse FromJson(string json) =>
            JsonConvert.DeserializeObject<FirebaseIdTokenResponse>(json, Firebase.Data.Converter.Settings);
    }
}