using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Dtos
{
    public class MoviePatchDto
    {
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}