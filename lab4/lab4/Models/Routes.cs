namespace lab4.Models
{
    public static class Routes
    {
        public const string GetMoviesList = "movies";
        public const string GetMovieById = "movie/{id}";
        public const string GetMoviesByGenre = "movies/genre={genre}";
        public const string GetMoviesByDirector = "movies/director={director}";
        public const string GetMoviesByYear = "movies/year={year}";
        public const string SetDatabase = "setDb";
        public const string Search = "search";
    }
}