using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Dtos;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly string containerName = "people";
        private readonly IFileStorageService fileStorageService;
        public PeopleController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        :base(context, mapper)
        {
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            this.context = context;
        }

        [HttpGet(Name= "GetPeople")]
        public async Task<ActionResult<List<PersonDto>>> GetPeople([FromQuery]PaginationDto pagination)
        {
            return await Get<Person, PersonDto>(pagination);
        }

        [HttpGet("{id}", Name = "GetPerson")]
        public async Task<ActionResult<PersonDto>> GetPerson(int id)
        {
            return await Get<Person, PersonDto>(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm]PersonForCreationDto personForCreationDto)
        {
            var person = mapper.Map<Person>(personForCreationDto);

            if (personForCreationDto.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personForCreationDto.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();

                    var extension = Path.GetExtension(personForCreationDto.Picture.FileName);

                    person.Picture = await fileStorageService.SaveFile(content, extension, containerName, personForCreationDto.Picture.ContentType);
                }
            }

            context.Add(person);
            await context.SaveChangesAsync();

            var personDto = mapper.Map<PersonDto>(person);

            return new CreatedAtRouteResult("GetPerson", new { id = person.Id }, personDto);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm]PersonForCreationDto personForCreationDto)
        {
            var personFromDb = await context.People.FirstOrDefaultAsync(p => p.Id == id);

            if (personFromDb == null)
                return NotFound();

            personFromDb = mapper.Map(personForCreationDto, personFromDb);

            if (personForCreationDto.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personForCreationDto.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();

                    var extension = Path.GetExtension(personForCreationDto.Picture.FileName);

                    personFromDb.Picture = await fileStorageService.EditFile(content, extension, containerName, personFromDb.Picture, personForCreationDto.Picture.ContentType);
                }
            }
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody]JsonPatchDocument<PersonPatchDto> patchDocument)
        {
            return await Patch<Person, PersonPatchDto>(id, patchDocument);

            // if (patchDocument == null)
            //     return BadRequest();

            // var entityFromDb = await context.People.FirstOrDefaultAsync(p => p.Id == id);

            // if (entityFromDb == null)
            //     return NotFound();

            // var entityDto = mapper.Map<PersonPatchDto>(entityFromDb);

            // patchDocument.ApplyTo(entityDto, ModelState);

            // var isValid = TryValidateModel(entityDto);

            // if  (!isValid)
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
            return await Delete<Person>(id);

        }
    }
}