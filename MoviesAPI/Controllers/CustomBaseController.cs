using System.Collections.Generic;
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

namespace MoviesAPI.Controllers 
{
    [ApiExplorerSettings (IgnoreApi = true)]
    public class CustomBaseController : ControllerBase 
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public CustomBaseController (ApplicationDbContext context, IMapper mapper) 
        {
            this.mapper = mapper;
            this.context = context;
        }

        protected async Task<List<TDto>> Get<TEntity, TDto>() where TEntity: class
        {
            var entities = await context.Set<TEntity>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDto>>(entities);

            return dtos;
        }

        protected async Task<List<TDto>> Get<TEntity, TDto>([FromQuery]PaginationDto pagination) where TEntity: class
        {
            var querable = context.Set<TEntity>().AsNoTracking().AsQueryable();

            await HttpContext.InsertPaginationParametersInResponse(querable, pagination.RecordsPerPage);

            var entities = await querable.Paginate(pagination).ToListAsync();

            return mapper.Map<List<TDto>>(entities);
        }

        protected async Task<ActionResult<TDto>> Get<TEntity, TDto>(int id) where TEntity: class, IId
        {
            var entity = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            var dto = mapper.Map<TDto>(entity);

            return dto;
        }

        protected async Task<ActionResult> Post<TCreation, TEntity, TRead>(TCreation creation, string routeName)where TEntity: class, IId
        {
            var entity = mapper.Map<TEntity>(creation);

            context.Add(entity);

            await context.SaveChangesAsync();

            var readDto = mapper.Map<TRead>(entity);

            return new CreatedAtRouteResult(routeName, new { id = entity.Id }, readDto);
        }

        protected async Task<ActionResult> Put<TCreation, TEntity>(int id, TCreation creation)where TEntity: class, IId
        {
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            var dto = mapper.Map(creation, entity);

            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity: class, IId
        {
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            context.Remove(entity);

            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntity, TDto>(int id, JsonPatchDocument<TDto> patchDocument) where TDto: class where TEntity: class, IId
        {
            if (patchDocument == null)
                return BadRequest();

            var entityFromDb = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDb == null)
                return NotFound();

            var entityDto = mapper.Map<TDto>(entityFromDb);

            patchDocument.ApplyTo(entityDto, ModelState);

            var isValid = TryValidateModel(entityDto);

            if (!isValid)
            {
                return BadRequest();
            }

            mapper.Map(entityDto, entityFromDb);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}