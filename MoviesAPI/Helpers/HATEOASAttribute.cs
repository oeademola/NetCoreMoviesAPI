using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoviesAPI.Helpers
{
    public class HATEOASAttribute: ResultFilterAttribute
    {
        protected bool ShouldIncludeHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            if (!IsSuccesfulResponse(result))
            {
                return false;
            }

            var header = context.HttpContext.Request.Headers["IncludeHATEOAS"];

            if (header.Count == 0)
            {
                return false;
            }

            var value = header[0];

            if (!value.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }

        private bool IsSuccesfulResponse(ObjectResult result)
        {
            if (result == null || result.Value == null)
            {
                return false;
            }

            if (result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }

            return true;
        }
    }
}