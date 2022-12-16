namespace Contracts.Book
{
    public class BookDetailsDto
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? PageCount { get; set; }
        public AuthorDetailsDto[] Authors { get; set; }
    }
}
