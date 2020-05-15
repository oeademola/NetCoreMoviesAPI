using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesAPI.Validations;

namespace MoviesAPI.Entities
{
    public class Genre: IId
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        [FirstLetterUppercase]
        public string Name { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
    }

}