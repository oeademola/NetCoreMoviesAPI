namespace MoviesAPI.Dtos
{
    public class FilterMoviesDto
    {
        public int Page { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 10;
        public PaginationDto Pagination
        {
            get {return new PaginationDto() {Page = Page, RecordsPerPage = RecordsPerPage};}
        }
        public string Title { get; set; }
        public int GenreId { get; set; }
        public bool InTheaters { get; set; }
        public bool UpcomingReleases { get; set; }
        public string OrderingField { get; set; }
        public bool AscendingOrder { get; set; } = true;
    }
}