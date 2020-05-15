using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class MoviesControllerTests: BaseTests
    {
        private string CreateTestsData()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var genre = new Genre() {Name = "Genre 1"};

            var movies = new List<Movie>()
            {
                new Movie() {Title = "Movie 1", ReleaseDate = new DateTime(2019, 1,1), InTheaters = false},
                new Movie() {Title = "Future Movie", ReleaseDate = DateTime.Today.AddDays(1), InTheaters = false},
                new Movie() {Title = "In theater movie", ReleaseDate = DateTime.Today.AddDays(-1), InTheaters = true}
            };

            var movieWithGenre = new Movie()
            {
                Title = "Movie with Genre",
                ReleaseDate = new DateTime(2019, 1, 1),
                InTheaters = false
            };
            movies.Add(movieWithGenre);

            context.Add(genre);
            context.AddRange(movies);
            context.SaveChanges();

            var movieGenre = new MoviesGenres() 
            {
                GenreId = genre.Id,
                MovieId = movieWithGenre.Id
            };

            context.Add(movieGenre);
            context.SaveChanges();

            return databaseName;
        }

        [TestMethod]
        public async Task FilterByTitle()
        {
            //Preparation
            var databaseName = CreateTestsData();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var filterMovieDto = new FilterMoviesDto()
            {
                Title = "Movie 1",
                RecordsPerPage = 10
            };

            //Testing
            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Filter(filterMovieDto);

            //Verification
            var result = response.Value;
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Movie 1", result[0].Title);

        }

        [TestMethod]
        public async Task FilterByInTheaters()
        {
            //Preparation
            var databaseName = CreateTestsData();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var filterMovieDto = new FilterMoviesDto()
            {
                InTheaters = true,
                RecordsPerPage = 10
            };

            //Testing
            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Filter(filterMovieDto);

            //Verification
            var result = response.Value;
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("In theater movie", result[0].Title);

        }

        [TestMethod]
        public async Task FilterByUpcomingRelease()
        {
            //Preparation
            var databaseName = CreateTestsData();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var filterMovieDto = new FilterMoviesDto()
            {
                UpcomingReleases = true,
                RecordsPerPage = 10
            };

            //Testing
            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Filter(filterMovieDto);

            //Verification
            var result = response.Value;
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Future Movie", result[0].Title);

        }

        [TestMethod]
        public async Task FilterByGenre()
        {
            //Preparation
            var databaseName = CreateTestsData();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var genreId = context.Genres.Select(x => x.Id).First();

            var filterMovieDto = new FilterMoviesDto()
            {
                GenreId = genreId,
                RecordsPerPage = 10
            };

            //Testing
            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Filter(filterMovieDto);

            //Verification
            var result = response.Value;
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Movie with Genre", result[0].Title);

        }

        [TestMethod]
        public async Task FilterOrderByTitleAscending()
        {
            //Preparation
            var databaseName = CreateTestsData();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var context2 = BuildContext(databaseName);

            var filterMovieDto = new FilterMoviesDto()
            {
                OrderingField = "title",
                AscendingOrder = true,
                RecordsPerPage = 10
            };

            //Testing
            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Filter(filterMovieDto);

            var moviesFromDb = context2.Movies.OrderBy(x => x.Title).ToList();

            //Verification
            var result = response.Value;
            Assert.AreEqual(moviesFromDb.Count, result.Count);

            for (int i = 0; i < moviesFromDb.Count; i++)
            {
                var movieFromController = result[i];
                var movieFromDb = moviesFromDb[i];

                Assert.AreEqual(movieFromDb.Id, movieFromController.Id);
            }


        }

        [TestMethod]
        public async Task FilterOrderByTitleDescending()
        {
            //Preparation
            var databaseName = CreateTestsData();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var context2 = BuildContext(databaseName);

            var filterMovieDto = new FilterMoviesDto()
            {
                OrderingField = "title",
                AscendingOrder = false,
                RecordsPerPage = 10
            };

            //Testing
            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = await controller.Filter(filterMovieDto);

            var moviesFromDb = context2.Movies.OrderByDescending(x => x.Title).ToList();

            //Verification
            var result = response.Value;
            Assert.AreEqual(moviesFromDb.Count, result.Count);

            for (int i = 0; i < moviesFromDb.Count; i++)
            {
                var movieFromController = result[i];
                var movieFromDb = moviesFromDb[i];

                Assert.AreEqual(movieFromDb.Id, movieFromController.Id);
            }


        }

        [TestMethod]
        public async Task FilterWrongOrderingFieldStillReturnsMovies()
        {
            //Preparation
            var databaseName = CreateTestsData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var context2 = BuildContext(databaseName);

            var mock = new Mock<ILogger<MoviesController>>();

            var filterMovieDto = new FilterMoviesDto()
            {
                OrderingField = "invalid field",
                AscendingOrder = false,
                RecordsPerPage = 10
            };

            //Testing
            var controller = new MoviesController(context, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Filter(filterMovieDto);

            //Verification 
            var result = response.Value;
            var moviesFromDb = context2.Movies.ToList();
            Assert.AreEqual(moviesFromDb.Count, result.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
    }
}