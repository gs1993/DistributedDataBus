namespace OrderService.Models
{
    public class Order
    {
        protected Order() { }
        public Order(int id, string name, string status)
        {
            Id = id;
            Name = name;
            Status = status;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }


}
