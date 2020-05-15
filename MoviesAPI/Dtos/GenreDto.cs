using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Dtos
{
    public class GenreDto: IGenerateHATEOASLinks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

        public void GenerateLinks(IUrlHelper urlHelper)
        {
            Links.Add(new Link(urlHelper.Link("GetGenre", new { id = Id }), rel: "get-genre", method: "GET"));
            Links.Add(new Link(urlHelper.Link("UpdateGenre", new { id = Id }), rel: "update-genre", method: "PUT"));
            Links.Add(new Link(urlHelper.Link("DeleteGenre", new { id = Id }), rel: "delete-genre", method: "DELETE"));
        }

        public ResourceCollection<GenreDto> GenerateLinksCollection<GenreDto>(List<GenreDto> dtos, IUrlHelper urlHelper)
        {
            var resourceCollection = new ResourceCollection<GenreDto>(dtos);
            resourceCollection.Links.Add(new Link(urlHelper.Link("GetGenres", new { }), rel: "self", method: "GET"));
            resourceCollection.Links.Add(new Link(urlHelper.Link("CreateGenre", new { }), rel: "create-genre", method: "POST"));

            return resourceCollection;
        }
    }
}