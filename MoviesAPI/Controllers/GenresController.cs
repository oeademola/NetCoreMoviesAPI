using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.Data;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;
using MoviesAPI.Filters;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ApplicationDbContext context, IMapper mapper)
        :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "GetGenres")]
        public async Task<ActionResult<List<GenreDto>>> GetGenres()
        {
            return await Get<Genre, GenreDto>();

           
            // if (includeHATEOAS)
            // {
            //     var resourceCollection = new ResourceCollection<GenreDto>(genreDto);
            //     genreDto.ForEach(genre => GenerateLinks(genre));
            //     resourceCollection.Links.Add(new Link(Url.Link("GetGenres", new { }), rel: "self", method: "GET"));
            //     resourceCollection.Links.Add(new Link(Url.Link("CreateGenre", new { }), rel: "create-genre", method: "POST"));
            //     return Ok(resourceCollection);
            // }
            // return Ok(genreDto);
        }

        // private void GenerateLinks(GenreDto genreDto)
        // {
        //     genreDto.Links.Add(new Link(Url.Link("GetGenre", new { id = genreDto.Id }), rel: "get-genre", method: "GET"));
        //     genreDto.Links.Add(new Link(Url.Link("UpdateGenre", new { id = genreDto.Id }), rel: "update-genre", method: "PUT"));
        //     genreDto.Links.Add(new Link(Url.Link("DeleteGenre", new { id = genreDto.Id }), rel: "delete-genre", method: "DELETE"));
        // }

        [HttpGet("{id}", Name = "GetGenre")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(GenreDto), 200)]
        public async Task<ActionResult<GenreDto>> GetGenre(int id)
        {
            return await Get<Genre, GenreDto>(id);

        }

        [HttpPost(Name = "CreateGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromBody] GenreForCreationDto genreForCreationDto)
        {
            return await Post<GenreForCreationDto, Genre, GenreDto>(genreForCreationDto, "GetGenre");
        }

        [HttpPut("{id}", Name = "UpdateGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreForCreationDto genreForCreationDto)
        {
            return await Put<GenreForCreationDto, Genre>(id, genreForCreationDto);
        }

        /// <summary>
        /// Delete a genre
        /// </summary>
        /// <param name="id">Id of the genre to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var name = User.Identity.Name;
            }
            return await Delete<Genre>(id);
        }
    }
}