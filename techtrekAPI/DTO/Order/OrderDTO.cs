using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace techtrekAPI.DTO.Order
{
    public class OrderDTO
    {
        [Key] public int id { get; set; }
        public int? user_id { get; set; }
        public string? order_items { get; set; }
        public float? total_cost { get; set; }
        [NotMapped] public string? first_name { get; set; }
        [NotMapped] public string? last_name { get; set; }
        public string? status { get; set; }
        public DateTime? created_at { get; set; }
    }
}
