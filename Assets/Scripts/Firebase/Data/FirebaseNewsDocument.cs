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
        public StringV PreviewImage { get; set; }
        
        [JsonProperty("author")]
        public StringV Author { get; set; }
        
        [JsonProperty("content")]
        public StringV Content { get; set; }
        
        [JsonProperty("title")]
        public StringV Title { get; set; }
        
        [JsonProperty("publishDate")]
        public DateV PublishDate { get; set; }
        
        [JsonProperty("url")]
        public StringV Url { get; set; }
        
        [JsonProperty("likes")]
        public IntV Likes { get; set; }
    }
    
    public class DateV
    {
        [JsonProperty("timestampValue")]
        public DateTimeOffset TimestampValue { get; set; }
    }
    
    public class IntV
    {
        [JsonProperty("integerValue")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IntegerValue { get; set; }
    }
    
    public class StringV
    {
        [JsonProperty("stringValue")]
        public string StringValue { get; set; }
    }

    public partial class FirebaseNewsDocument
    {
        public static FirebaseNewsDocument FromJson(string json) => JsonConvert.DeserializeObject<FirebaseNewsDocument>(json, Firebase.Data.Converter.Settings);
    }
}