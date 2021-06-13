using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Firebase.Data
{
    public partial class Number
    {
        [JsonProperty("integerValue")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IntegerValue { get; set; }

        public Number()
        {
            IntegerValue = 0;
        }
    }

    public partial class String
    {
        [JsonProperty("stringValue")] public string StringValue { get; set; }

        public String()
        {
            StringValue = "";
        }
    }
    
    public class Date
    {
        [JsonProperty("timestampValue")]
        public DateTimeOffset TimestampValue { get; set; }

        public Date()
        {
            TimestampValue = DateTimeOffset.Now;
        }
    }
    
    public class FirestoreStringList
    {
        [JsonProperty("arrayValue")]
        public StringListArrayValue ArrayValue { get; set; }

        public FirestoreStringList()
        {

            ArrayValue = new StringListArrayValue
            {
                Values = new List<String>()
            };
        }
    }

    public class StringListArrayValue
    {
        [JsonProperty("values")]
        public List<String> Values { get; set; }
    }
}