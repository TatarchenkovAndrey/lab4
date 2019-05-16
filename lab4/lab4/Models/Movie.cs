using System.Collections.Generic;
using Newtonsoft.Json;

namespace lab4.Models
{
    public class Movie 
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "rating")]
        public int Rating { get; set; }
        
        [JsonProperty(PropertyName = "directors")]
        public IEnumerable<string> Directors { get; set; }
        
        [JsonProperty(PropertyName = "genres")]
        public IEnumerable<string> Genres { get; set; }
        
        [JsonProperty(PropertyName = "release_dates")]
        public IEnumerable<string> ReleaseDates { get; set; }
        
        [JsonProperty(PropertyName = "storyline")]
        public string StoryLine { get; set; }
        
        [JsonProperty(PropertyName = "synopsis")]
        public string Synopsis { get; set; }
        
        [JsonProperty(PropertyName = "top_3_cast")]
        public IEnumerable<string> Top3Cast { get; set; }
        
        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }
    }

    public class MovieItem
    {
        [JsonProperty(PropertyName = "movie")]
        public IEnumerable<Movie> Movies { get; set; }
    }

}