using DataBus.Requests.Order;

namespace DataBus.Requests.Book
{
    public record ImportBookRequest : IRequest
    {
        public ImportBookRequest(string isbn)
        {
            Isbn = isbn;
        }

        public string Isbn { get; init; }
    }
}
