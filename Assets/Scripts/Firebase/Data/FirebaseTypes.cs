using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Firebase.Data
{
    public partial class Number
    {
        [JsonProperty("integerValue")]
        [JsonConverter(typeof(ParseStringConverter))]
        public Nullable<long> IntegerValue { get; set; }

        public Number()
        {
            IntegerValue = null;
        }
    }

    public partial class String
    {
        [JsonProperty("stringValue")] public string StringValue { get; set; }

        public String()
        {
            StringValue = null;
        }
    }
    
    public class Date
    {
        [JsonProperty("timestampValue")]
        public Nullable<DateTimeOffset> TimestampValue { get; set; }

        public Date()
        {
            TimestampValue = null;
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