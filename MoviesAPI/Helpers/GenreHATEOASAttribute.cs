using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using MoviesAPI.Dtos;
using MoviesAPI.Services;

namespace MoviesAPI.Helpers {
    public class GenreHATEOASAttribute : HATEOASAttribute {
        private readonly LinksGenerator linksGenerator;
        public GenreHATEOASAttribute (LinksGenerator linksGenerator) 
        {
            this.linksGenerator = linksGenerator;

        }
        public override async Task OnResultExecutionAsync (ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var includeHATEOAS = ShouldIncludeHATEOAS (context);

            if (!includeHATEOAS) 
            {
                await next ();
                return;
            }

            await linksGenerator.Generate<GenreDto>(context, next);
        }
    }
}