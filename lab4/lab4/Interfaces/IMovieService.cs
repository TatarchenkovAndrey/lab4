using System.Collections.Generic;
using System.Threading.Tasks;
using lab4.Models;

namespace lab4.Interfaces
{
    public interface IMovieService
    {
        ValueTask<MovieItem> Movie(string id);
        ValueTask<Data> Movies();
        ValueTask<Data> MovieByYear(int year);
        ValueTask<Data> MovieByDirector(string director);
        ValueTask<Data> MovieByGenre(string genre);
    }
}