using Refit;

namespace BookSearchService.ApiClients.OpenLibrary
{
    public interface IOpenLibraryApi
    {
        [Get("/isbn/{isbn}.json")]
        Task<BookDto> GetBook(string isbn, CancellationToken cancellationToken);

        [Get("/{key}.json")]
        Task<AuthorDetailsDto> GetAuthor(string key, CancellationToken cancellationToken);
    }

    public class BookDto
    {
        public List<string> Isbn_10 { get; set; }
        public string Title { get; set; }
        public string Publish_date { get; set; }
        public int Number_of_pages { get; set; }
        public List<AuthorDto> Authors { get; set; }
    }

    public class AuthorDto
    {
        public string Key { get; set; }
    }

    public class AuthorDetailsDto
    {
        public string Name { get; set; }
        public string Bio { get; set; }
    }
}
