namespace Firebase.Data
{
    using System;
    using System.Collections.Generic;
    
    using Newtonsoft.Json;
    
    public partial class FirebaseNewsDocumentAll
    {
        [JsonProperty("documents")]
        public FirebaseNewsDocument[] Documents;
    }
    
    public partial class FirebaseNewsDocumentAll
    {
        public static FirebaseNewsDocumentAll FromJson(string json) => JsonConvert.DeserializeObject<FirebaseNewsDocumentAll>(json, Firebase.Data.Converter.Settings);
    }
}