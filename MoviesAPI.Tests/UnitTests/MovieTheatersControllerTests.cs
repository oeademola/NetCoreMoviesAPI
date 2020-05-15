using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Tests.UnitTests
{
    // [TestClass]
    // public class MovieTheatersControllerTests : BaseTests
    // {
    //     [TestMethod]
    //     public async Task GetMovieTheaters50KmsOrCloser()
    //     {
    //         var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    //         using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb())
    //         {
    //             var theaters = new List<MovieTheater>
    //                 {
    //                     new MovieTheater{Id = 1, Name = "SilverBird Cinema", Location = geometryFactory.CreatePoint(new Coordinate(3.3154061, 6.8294238))},
    //                     new MovieTheater{Id = 2, Name = "Lekki Film House", Location = geometryFactory.CreatePoint(new Coordinate(3.5069803, 6.4375493))},
    //                     new MovieTheater{Id = 3, Name = "Ozone Cinema", Location = geometryFactory.CreatePoint(new Coordinate(3.3393472, 6.5062713))},
    //                     new MovieTheater{Id = 4, Name = "CineWorld", Location = geometryFactory.CreatePoint(new Coordinate(-7.4792365, 53.5731389))}
    //                 };

    //             context.AddRange(theaters);
    //             context.SaveChanges();
    //         }

    //         var filterMovieTheatersDto = new FilterMovieTheatersDto()
    //         {
    //             DistanceInKms = 50,
    //             Lat = 6.490575,
    //             Long = 3.606931  
    //         };

    //         using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb())
    //         {
    //             var controller = new MovieTheatersController(context);
    //             var response = await controller.Get(filterMovieTheatersDto);
    //             var result = response.Value;

    //             Assert.AreEqual(2, result.Count);
    //         }

    //     }
    // }
}