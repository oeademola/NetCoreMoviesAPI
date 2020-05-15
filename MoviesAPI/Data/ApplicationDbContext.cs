using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesGenres>().HasKey(x => new {x.GenreId, x.MovieId});
            modelBuilder.Entity<MoviesActors>().HasKey(x => new {x.PersonId, x.MovieId});

            SeedData(modelBuilder);
             
            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var geometryFactory= NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            modelBuilder.Entity<MovieTheater>()
                .HasData(new List<MovieTheater>
                {
                    new MovieTheater{Id = 1, Name = "SilverBird Cinema", Location = geometryFactory.CreatePoint(new Coordinate(3.3154061, 6.8294238))},
                    new MovieTheater{Id = 2, Name = "Lekki Film House", Location = geometryFactory.CreatePoint(new Coordinate(3.5069803, 6.4375493))},
                    new MovieTheater{Id = 3, Name = "Ozone Cinema", Location = geometryFactory.CreatePoint(new Coordinate(3.3393472, 6.5062713))},
                    new MovieTheater{Id = 4, Name = "CineWorld", Location = geometryFactory.CreatePoint(new Coordinate(-7.4792365, 53.5731389))}
                });

            var adventure = new Genre() { Id = 4, Name = "Adventure" };
            var animation = new Genre() { Id = 5, Name = "Animation" };
            var drama = new Genre() { Id = 6, Name = "Drama" };
            var romance = new Genre() { Id = 7, Name = "Romance" };

            modelBuilder.Entity<Genre>()
                .HasData(new List<Genre>
                {
                    adventure, animation, drama, romance
                });

            var jimCarrey = new Person() { Id = 5, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 01, 17) };
            var robertDowney = new Person() { Id = 6, Name = "Robert Downey Jr.", DateOfBirth = new DateTime(1965, 4, 4) };
            var chrisEvans = new Person() { Id = 7, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Person>()
                .HasData(new List<Person>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Movie()
            {
                Id = 2,
                Title = "Avengers: Endgame",
                InTheaters = true,
                ReleaseDate = new DateTime(2019, 04, 26)
            };
            var sonic = new Movie()
            {
                Id = 4,
                Title = "Sonic the Hedgehog",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 28)
            };
            var emma = new Movie()
            {
                Id = 5,
                Title = "Emma",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };
            var greed = new Movie()
            {
                Id = 6,
                Title = "Greed",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };
            var iw = new Movie()
            {
                Id = 7,
                Title = "Avengers: Infinity Wars",
                InTheaters = false,
                ReleaseDate = new DateTime(2019, 04, 26)
            };


            modelBuilder.Entity<Movie>()
                .HasData(new List<Movie>
                {
                    endgame, iw, sonic, emma, greed
                });

            modelBuilder.Entity<MoviesGenres>().HasData(
                new List<MoviesGenres>()
                {
                    new MoviesGenres(){MovieId = endgame.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = endgame.Id, GenreId = adventure.Id},
                    new MoviesGenres(){MovieId = iw.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = iw.Id, GenreId = adventure.Id},
                    new MoviesGenres(){MovieId = sonic.Id, GenreId = adventure.Id},
                    new MoviesGenres(){MovieId = emma.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = emma.Id, GenreId = romance.Id},
                    new MoviesGenres(){MovieId = greed.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = greed.Id, GenreId = romance.Id},
                });

            modelBuilder.Entity<MoviesActors>().HasData(
                new List<MoviesActors>()
                {
                    new MoviesActors(){MovieId = endgame.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = endgame.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = iw.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = iw.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = sonic.Id, PersonId = jimCarrey.Id, Character = "Dr. Ivo Robotnik", Order = 1}
                });
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
        public DbSet<MovieTheater> MovieTheaters { get; set; }
    }
}