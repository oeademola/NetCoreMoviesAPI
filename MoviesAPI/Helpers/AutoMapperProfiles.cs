using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDto>().ReverseMap();
            
            CreateMap<GenreForCreationDto, Genre>();
            
            CreateMap<Person, PersonDto>().ReverseMap();

            CreateMap<PersonForCreationDto, Person>()
                .ForMember(p => p.Picture, options => options.Ignore());

            CreateMap<Person, PersonPatchDto>().ReverseMap();

            CreateMap<Movie, MovieDto>().ReverseMap();

            CreateMap<MovieForCreationDto, Movie>()
                .ForMember(m => m.Poster, options => options.Ignore())
                .ForMember(m => m.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(m => m.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDetailsDto>()
                .ForMember(m => m.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(m => m.Actors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MoviePatchDto>().ReverseMap();
            
            CreateMap<IdentityUser, UserDto>()
                .ForMember(u => u.EmailAddress, options => options.MapFrom(u => u.Email))
                .ForMember(u => u.UserId, options => options.MapFrom(u => u.Id));
        }


        private List<GenreDto> MapMoviesGenres(Movie movie, MovieDetailsDto movieDetailsDto)
        {
            var result = new List<GenreDto>();
            foreach (var moviegenre in movie.MoviesGenres)
            {
                result.Add(new GenreDto() {Id = moviegenre.GenreId, Name = moviegenre.Genre.Name});
            }

            return result;
        }

        private List<ActorDto> MapMoviesActors(Movie movie, MovieDetailsDto movieDetailsDto)
        {
            var result = new List<ActorDto>();
            foreach (var actor in movie.MoviesActors)
            {
                result.Add(new ActorDto() {PersonId = actor.PersonId, Character = actor.Character, PersonName = actor.Person.Name});
            }

            return result;
        }

        private List<MoviesGenres> MapMoviesGenres(MovieForCreationDto movieForCreationDto, Movie movie)
        {
            var result = new List<MoviesGenres>();
            foreach (var id in movieForCreationDto.GenreIds)
            {
                result.Add(new MoviesGenres() {GenreId = id});
            }

            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieForCreationDto movieForCreationDto, Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach (var actor in movieForCreationDto.Actors)
            {
                result.Add(new MoviesActors() {PersonId = actor.PersonId, Character = actor.Character});
            }

            return result;
        }
    }
}