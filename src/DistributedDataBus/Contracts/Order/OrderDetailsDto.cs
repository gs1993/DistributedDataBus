using Dawn;

namespace Contracts.Order.Models
{
    public record OrderDetailsDto
    {
        public OrderDetailsDto(OrderDetails grpcModel)
        {
            Guard.Argument(grpcModel).NotNull();

            Id = grpcModel.Id;
            Name = grpcModel.Name;
            Status = grpcModel.Status;
        }

        public int Id { get; init; }
        public string Name { get; init; }
        public string Status { get; init; }
    }
}
