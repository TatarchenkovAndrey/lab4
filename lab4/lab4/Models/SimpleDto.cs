using System.Collections.Generic;
using Newtonsoft.Json;

namespace lab4.Models
{
    
    public class SimpleDto
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "movies")]
        public IEnumerable<SimpleDto> Movies { get; set; }
    }
}