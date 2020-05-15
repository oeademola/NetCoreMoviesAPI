using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class GenresControllerTests: BaseTests
    {
        [TestMethod]
        public async Task GetAllGenres()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Genre() {Name = "Genre 1"});
            context.Genres.Add(new Genre() {Name = "Genre 2"});
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            //Testing
            var controller = new GenresController(context2, mapper);
            var response = await controller.GetGenres();

            //Verification
            var genres = response.Value;
            Assert.AreEqual(2, genres.Count);

        }

        [TestMethod]
        public async Task GetGenreByIdDoesNotExist()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var id = 1;

            //Testing
            var controller = new GenresController(context, mapper);
            var response = await controller.GetGenre(id);

            //Verification
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetGenreByIdSuccessfully()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Genre() {Name = "Genre 1"});
            context.Genres.Add(new Genre() {Name = "Genre 2"});
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            //Testing
            var id = 1;
            var controller = new GenresController(context2, mapper);
            var response = await controller.GetGenre(id);

            //Verification
            var result = response.Value;
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public async Task CreateGenre()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var newGenre = new GenreForCreationDto() {Name = "New Genre"};

            //Testing
            var controller = new GenresController(context, mapper);
            var response = await controller.Post(newGenre);

            //Verification
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context2 = BuildContext(databaseName);
            var count = await context2.Genres.CountAsync();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task UpdateGenre()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Genre() {Name = "Genre 1"});
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var genreForCreationDto = new GenreForCreationDto() {Name = "New Genre"};

            //Testing
            var id = 1;
            var controller = new GenresController(context, mapper);
            var response = await controller.Put(id, genreForCreationDto);

            //Verification
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var isExist = await context3.Genres.AnyAsync(x => x.Name == "New Genre");
            Assert.IsTrue(isExist);
            
        }

        [TestMethod]
        public async Task DeleteGenreNotFound()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            //Testing
            var controller = new GenresController(context, mapper);
            controller.ControllerContext = BuildControllerContextWithDefaultUser();
            var response = await controller.Delete(1);

            //Verification
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);

        }

        [TestMethod]
        public async Task DeleteGenreSuccessfully()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            context.Genres.Add(new Genre() {Name = "Genre 1"});
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            //Testing
            var controller = new GenresController(context2, mapper);
            controller.ControllerContext = BuildControllerContextWithDefaultUser();
            var response = await controller.Delete(1);

            //Verification
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var isExist = await context3.Genres.AnyAsync();
            Assert.IsFalse(isExist);

        }

    }
}