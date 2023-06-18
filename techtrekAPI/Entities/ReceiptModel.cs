namespace techtrekAPI.Entities
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalCost { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
    
        public string Name { get; set; }
        public int qty { get; set; }
        public decimal price { get; set; }
    }
}
