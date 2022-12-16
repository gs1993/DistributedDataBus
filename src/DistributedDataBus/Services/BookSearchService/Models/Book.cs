namespace BookSearchService.Models
{
    public class Book
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? PageCount { get; set; }
        public Author[] Authors { get; set; }
    }
}
