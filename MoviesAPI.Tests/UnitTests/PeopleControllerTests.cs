using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class PeopleControllerTests: BaseTests
    {
        [TestMethod]
        public async Task GetPeoplePaginated()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.People.Add(new Person() {Name = "Person 1"});
            context.People.Add(new Person() {Name = "Person 2"});
            context.People.Add(new Person() {Name = "Person 3"});
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            //Testing
            var controller = new PeopleController(context2, mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var responsePage1 = await controller.GetPeople(new PaginationDto() {Page = 1, RecordsPerPage = 2});

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var responsePage2 = await controller.GetPeople(new PaginationDto() {Page = 2, RecordsPerPage = 2});

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var responsePage3 = await controller.GetPeople(new PaginationDto() {Page = 3, RecordsPerPage = 2});

            //Verification
            var peoplePage1 = responsePage1.Value;
            Assert.AreEqual(2, peoplePage1.Count);

            var peoplePage2 = responsePage2.Value;
            Assert.AreEqual(1, peoplePage2.Count);

            var peoplePage3 = responsePage3.Value;
            Assert.AreEqual(0, peoplePage3.Count);

        }

        [TestMethod]
        public async Task CreatePersonWithoutImage()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var newPerson = new PersonForCreationDto() {Name = "New Person", Biography = "abc", DateOfBirth = DateTime.Now};

            var mock = new Mock<IFileStorageService>();
            mock.Setup(x => x.SaveFile(null, null, null, null))
                .Returns(Task.FromResult("url"));

            //Testing
            var controller = new PeopleController(context, mapper, null);
            var response = await controller.Post(newPerson);

            //Verification
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context2 = BuildContext(databaseName);
            var list = await context2.People.ToListAsync();
            Assert.AreEqual(1, list.Count);
            Assert.IsNull(list[0].Picture);

            Assert.AreEqual(0, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CreatePersonWithImage()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var content = Encoding.UTF8.GetBytes("This is a dummy image");
            var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "dummy.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";

            var newPerson = new PersonForCreationDto() 
            {
                Name = "New Person", 
                Biography = "abc", 
                DateOfBirth = DateTime.Now,
                Picture = file
            };

            var mock = new Mock<IFileStorageService>();
            mock.Setup(x => x.SaveFile(content, ".jpg", "people", file.ContentType))
                .Returns(Task.FromResult("url"));

            //Testing
            var controller = new PeopleController(context, mapper, mock.Object);
            var response = await controller.Post(newPerson);

            //Verification
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context2 = BuildContext(databaseName);
            var list = await context2.People.ToListAsync();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("url", list[0].Picture);

            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task PatchReturnsNotFound()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            //Testing
            var patchDocument = new JsonPatchDocument<PersonPatchDto>();
            var controller = new PeopleController(context, mapper, null);
            var response = await controller.Patch(1, patchDocument);

            //Verification 
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);

        }

        [TestMethod]
        public async Task Patch()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var dateOfBirth = DateTime.Now;
            var person = new Person() {Name = "Person 1", Biography = "abc", DateOfBirth = dateOfBirth};
            context.Add(person);
            await context.SaveChangesAsync();

            var context2 = BuildContext(databaseName);
            var patchDocument = new JsonPatchDocument<PersonPatchDto>();
            patchDocument.Operations.Add(new Operation<PersonPatchDto>("replace", "/name", null, "new value"));
            
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate
            (
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()
            ));

            //Testing
            var controller = new PeopleController(context2, mapper, null);
            controller.ObjectValidator = objectValidator.Object; 
            var response = await controller.Patch(1, patchDocument);

            //Verification 
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var personFromDb = await context3.People.FirstAsync();
            Assert.AreEqual("new value", personFromDb.Name);
            Assert.AreEqual("abc", personFromDb.Biography);
            Assert.AreEqual(dateOfBirth, personFromDb.DateOfBirth);

        }

    }
}