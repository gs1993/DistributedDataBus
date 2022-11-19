namespace DataBus.Requests
{
    public record CancelOrderRequest : IRequest
    {
        public CancelOrderRequest(int id)
        {
            Id = id;
        }

        public int Id { get; init; }
    }
}
