using System.Collections.Generic;

namespace MoviesAPI.Dtos
{
    public class ResourceCollection<T>
    {
        public List<T> Values { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

        public ResourceCollection(List<T> values)
        {
            Values = values;
        }
    }
}