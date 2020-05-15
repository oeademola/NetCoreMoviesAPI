using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Dtos
{
    public class PersonPatchDto
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}