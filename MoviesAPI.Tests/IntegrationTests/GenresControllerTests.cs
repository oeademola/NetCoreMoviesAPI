using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;
using Newtonsoft.Json;

namespace MoviesAPI.Tests.IntegrationTests
{
    [TestClass]
    public class GenresControllerTests: BaseTests
    {
        [TestMethod]
        public async Task GetGenresEmptyList()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);
            var client = factory.CreateClient();
            var url = "/api/Genres";


            //Testing
            var response = await client.GetAsync(url);

            //Verification
            response.EnsureSuccessStatusCode();
            var genres = JsonConvert.DeserializeObject<List<GenreDto>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(0, genres.Count);

        }

        [TestMethod]
        public async Task GetAllGenres()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);
            var client = factory.CreateClient();
            var url = "/api/Genres";
            var context = BuildContext(databaseName);

            context.Genres.Add(new Genre() {Name = "Genre 1"});
            context.Genres.Add(new Genre() {Name = "Genre 2"});

            context.SaveChanges();

            //Testing
            var response = await client.GetAsync(url);

            //Verification
            response.EnsureSuccessStatusCode();
            var genres = JsonConvert.DeserializeObject<List<GenreDto>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(2, genres.Count);

        }

        [TestMethod]
        public async Task DeleteGenre()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);
            var client = factory.CreateClient();
            var url = "/api/Genres";
            var context = BuildContext(databaseName);
            var context2 = BuildContext(databaseName);


            context.Genres.Add(new Genre() {Name = "Genre 1"});

            context.SaveChanges();

            //Testing
            var response = await client.DeleteAsync($"{url}/1");

            //Verification
            response.EnsureSuccessStatusCode();
            var isExist = await context2.Genres.AnyAsync();
            Assert.IsFalse(isExist);

        }

         [TestMethod]
        public async Task DeleteGenreIsProtected()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName, false);
            var client = factory.CreateClient();
            var url = "/api/Genres/1";

            //Testing
            var response = await client.DeleteAsync(url);

            //Verification
            Assert.AreEqual("Unauthorized", response.ReasonPhrase);

        }
    }
}