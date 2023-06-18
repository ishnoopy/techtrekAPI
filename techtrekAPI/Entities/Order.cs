using System.ComponentModel.DataAnnotations;

namespace techtrekAPI.Entities
{
    public class Order
    {
        [Key] public int id { get; set; }
        public int? user_id { get; set; }
        public float? total_cost { get; set; }
        public string? status { get; set; }
        public string? order_items { get; set; }
        public DateTime? created_at { get; set; }
    }
}
