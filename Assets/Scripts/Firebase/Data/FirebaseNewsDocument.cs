namespace Firebase.Data
{
    using System;
    using System.Collections.Generic;
    
    using Newtonsoft.Json;
    
    public partial class FirebaseNewsDocument
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("fields")]
        public NewsFields Fields { get; set; }
        
        [JsonProperty("createTime")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("updateTime")]
        public DateTimeOffset UpdateTime { get; set; }
    }

    public class NewsFields
    {
        [JsonProperty("previewImage")]
        public String PreviewImage { get; set; }
        
        [JsonProperty("author")]
        public String Author { get; set; }
        
        [JsonProperty("content")]
        public String Content { get; set; }
        
        [JsonProperty("title")]
        public String Title { get; set; }
        
        [JsonProperty("publishDate")]
        public Date PublishDate { get; set; }
        
        [JsonProperty("url")]
        public String Url { get; set; }
        
        [JsonProperty("likes")]
        public Number Likes { get; set; }
    }

    public partial class FirebaseNewsDocument
    {
        public static FirebaseNewsDocument FromJson(string json) => JsonConvert.DeserializeObject<FirebaseNewsDocument>(json, Firebase.Data.Converter.Settings);
    }
}