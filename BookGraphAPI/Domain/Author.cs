using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookGraphAPI.Domain
{
    public class Author
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; } 
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
    }
}
