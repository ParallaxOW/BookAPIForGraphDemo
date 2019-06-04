using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookGraphAPI.Domain
{
    public class Book
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("authorId")]
        public Guid? AuthorId { get; set; }

    }
}
