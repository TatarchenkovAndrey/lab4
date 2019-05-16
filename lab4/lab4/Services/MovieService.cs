using System.Collections.Generic;
using System.Threading.Tasks;
using lab4.Interfaces;
using lab4.Models;
using RestSharp;

namespace lab4.Services
{
    public class MovieService : IMovieService
    {
        public string url = "http://db.mirvoda.com/";
        
        public async ValueTask<MovieItem> Movie(string id)
        {
            var client = new RestClient(url);
            var request = new RestRequest($"movies/{id}", Method.GET);
            var result = client.Execute<MovieItem>(request).Data;
            return result;
        }

        public async ValueTask<Data> Movies()
        {
            var client = new RestClient(url);
            var request = new RestRequest($"movies", Method.GET);
            var result = client.Execute<Data>(request).Data;
            return result;
        }

        public async ValueTask<Data> MovieByYear(int year)
        {
            var client = new RestClient(url);
            var request = new RestRequest($"movies?year={year}", Method.GET);
            var result = client.Execute<Data>(request).Data;
            return result;
        }

        public async ValueTask<Data> MovieByDirector(string director)
        {
            var client = new RestClient(url);
            var request = new RestRequest($"movies?director={director}", Method.GET);
            var result = client.Execute<Data>(request).Data;
            return result;
        }

        public async ValueTask<Data> MovieByGenre(string genre)
        {
            var client = new RestClient(url);
            var request = new RestRequest($"movies?genre={genre}", Method.GET);
            var result = client.Execute<Data>(request).Data;
            return result;
        }

        
    }
}