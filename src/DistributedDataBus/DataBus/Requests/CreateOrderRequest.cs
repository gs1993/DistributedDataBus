namespace DataBus.Requests
{
    public interface IRequest 
    {
    }
    public record CreateOrderRequest : IRequest
    {
        public string Name { get; init; }

        public CreateOrderRequest(string name)
        {
            Name = name;
        }
    }
}
