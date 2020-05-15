using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "movies";
        private readonly ILogger<MoviesController> logger;

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService, ILogger<MoviesController> logger)
        :base(context, mapper)
        {
            this.logger = logger;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            this.context = context;

        }

        [HttpGet]
        public async Task<ActionResult<IndexMoviePageDto>> GetMovies()
        {
            var top = 7;
            var today = DateTime.Today;
            var upcomingRelease = await context.Movies
                .Where(m => m.ReleaseDate > today)
                .OrderBy(m => m.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var inTheaters = await context.Movies
                .Where(m => m.InTheaters)
                .Take(top)
                .ToListAsync();

            var result = new IndexMoviePageDto();
            result.InTheaters = mapper.Map<List<MovieDto>>(inTheaters);
            result.UpcomingRelease = mapper.Map<List<MovieDto>>(upcomingRelease);

            return result;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDto>>> Filter([FromQuery]FilterMoviesDto filterMoviesDto)
        {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDto.Title))
            {
                moviesQueryable = moviesQueryable.Where(m => m.Title.Contains(filterMoviesDto.Title));
            }

            if (filterMoviesDto.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(m => m.InTheaters);
            }

            if (filterMoviesDto.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(m => m.ReleaseDate > today);
            }

            if (filterMoviesDto.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(m => m.MoviesGenres.Select(x => x.GenreId)
                    .Contains(filterMoviesDto.GenreId));
            }

            if (!string.IsNullOrWhiteSpace(filterMoviesDto.OrderingField))
            {
                try
                {
                    moviesQueryable = moviesQueryable
                    .OrderBy($"{filterMoviesDto.OrderingField} {(filterMoviesDto.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    //log error
                    logger.LogWarning("Could not order by field: " + filterMoviesDto.OrderingField);
                }
                
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable, filterMoviesDto.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMoviesDto.Pagination).ToListAsync();

            return mapper.Map<List<MovieDto>>(movies);
        }

        [HttpGet("{id}", Name = "GetMovie")]
        public async Task<ActionResult<MovieDetailsDto>> GetMovie(int id)
        {
            var movie = await context.Movies
                .Include(m => m.MoviesActors).ThenInclude(m => m.Person)
                .Include(m => m.MoviesGenres).ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            var movieDto = mapper.Map<MovieDetailsDto>(movie);

            return movieDto;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm]MovieForCreationDto movieForCreationDto)
        {
            var movie = mapper.Map<Movie>(movieForCreationDto);

            if (movieForCreationDto.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieForCreationDto.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();

                    var extension = Path.GetExtension(movieForCreationDto.Poster.FileName);

                    movie.Poster = await fileStorageService.SaveFile(content, extension, containerName, movieForCreationDto.Poster.ContentType);
                }
            }

            AnnotateActorsOrder(movie);
            context.Add(movie);
            await context.SaveChangesAsync();

            var movieDto = mapper.Map<MovieDto>(movie);

            return new CreatedAtRouteResult("GetMovie", new { id = movie.Id }, movieDto);
        }

        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm]MovieForCreationDto movieForCreationDto)
        {
            var movieFromDb = await context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if (movieFromDb == null)
                return NotFound();

            movieFromDb = mapper.Map(movieForCreationDto, movieFromDb);

            if (movieForCreationDto.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieForCreationDto.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();

                    var extension = Path.GetExtension(movieForCreationDto.Poster.FileName);

                    movieFromDb.Poster = await fileStorageService.SaveFile(content, extension, containerName, movieForCreationDto.Poster.ContentType);
                }
            }

            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movieFromDb.Id}; delete from MoviesGenres where MovieId = {movieFromDb.Id}");
            AnnotateActorsOrder(movieFromDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody]JsonPatchDocument<MoviePatchDto> patchDocument)
        {

            return await Patch<Movie, MoviePatchDto>(id, patchDocument);

            // if (patchDocument == null)
            //     return BadRequest();

            // var entityFromDb = await context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            // if (entityFromDb == null)
            //     return NotFound();

            // var entityDto = mapper.Map<MoviePatchDto>(entityFromDb);

            // patchDocument.ApplyTo(entityDto, ModelState);

            // var isValid = TryValidateModel(entityDto);

            // if (!isValid)
            // {
            //     return BadRequest();
            // }

            // mapper.Map(entityDto, entityFromDb);
            // await context.SaveChangesAsync();

            // return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Movie>(id);

        }
    }
}