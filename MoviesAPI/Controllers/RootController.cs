using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Dtos;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet("GetRoot")]
        public ActionResult<IEnumerable<Link>> GetRoot()
        {
            List<Link> links = new List<Link>();

            links.Add(new Link(href: Url.Link("", new {}), rel: "self", method: "GET"));
            links.Add(new Link(href: Url.Link("CreateUser", new {}), rel: "create-user", method: "POST"));
            links.Add(new Link(href: Url.Link("Login", new {}), rel: "login", method: "POST"));
            links.Add(new Link(href: Url.Link("GetGenres", new {}), rel: "get-genres", method: "GET"));
            links.Add(new Link(href: Url.Link("GetPeople", new {}), rel: "get-people", method: "GET"));

            return links;
        }
    }
}