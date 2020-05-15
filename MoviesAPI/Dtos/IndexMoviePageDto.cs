using System.Collections.Generic;

namespace MoviesAPI.Dtos
{
    public class IndexMoviePageDto
    {
        public List<MovieDto> UpcomingRelease { get; set; }
        public List<MovieDto> InTheaters { get; set; }
    }
}