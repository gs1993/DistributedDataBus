namespace Contracts.Book
{
    public record ImportBatchBookDto
    {
        public string[] Isbns { get; init; }
    }
}
