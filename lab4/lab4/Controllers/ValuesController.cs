using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lab4.Interfaces;
using lab4.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IMovieService _service { get; set; }

        public ValuesController(IMovieService movieService)
        {
            _service = movieService;
        }
        
        [HttpGet]
        [Route(Routes.GetMoviesList)]
        public async ValueTask<Data> Get()
        {
            return await _service.Movies();
        }

        [HttpGet]
        [Route(Routes.GetMovieById)]
        public async ValueTask<MovieItem> Movies(string id)
        {   
            return await _service.Movie(id);
        }

        [HttpGet]
        [Route(Routes.GetMoviesByGenre)]
        public async ValueTask<Data> MoviesByGenre( string genre)
        {
            return await _service.MovieByGenre(genre);
        }

        [HttpGet]
        [Route(Routes.GetMoviesByYear)]
        public async ValueTask<Data> MovieByYear(int year)
        {
            return await _service.MovieByYear(year);
        }
        
        [HttpGet]
        [Route(Routes.GetMoviesByDirector)]
        public async ValueTask<Data> MoviesByDirector( string director)
        {
            return await _service.MovieByDirector(director);
        }

    }
}
