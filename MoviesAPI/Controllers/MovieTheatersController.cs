using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Dtos;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieTheatersController: ControllerBase 
    {
        private readonly ApplicationDbContext context;
        public MovieTheatersController (ApplicationDbContext context) 
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDto>>> Get([FromQuery]FilterMovieTheatersDto filterMovieTheatersDto)
        {
            var geometryFactory= NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var usersLocation = geometryFactory
                .CreatePoint(new Coordinate(filterMovieTheatersDto.Long, filterMovieTheatersDto.Lat));

            var theaters = await context.MovieTheaters
                .OrderBy(x => x.Location.Distance(usersLocation))
                .Where(x => x.Location.IsWithinDistance(usersLocation, filterMovieTheatersDto.DistanceInKms * 1000))
                .Select(x => new MovieTheaterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DistanceInMeters = Math.Round(x.Location.Distance(usersLocation))
                }).ToListAsync();

            return theaters;
        }
    }
}