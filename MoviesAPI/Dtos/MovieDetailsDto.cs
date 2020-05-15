using System.Collections.Generic;

namespace MoviesAPI.Dtos
{
    public class MovieDetailsDto : MovieDto
    {
        public List<GenreDto> Genres { get; set; }
        public List<ActorDto> Actors { get; set; }
    }
}