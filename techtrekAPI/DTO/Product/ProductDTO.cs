using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace techtrekAPI.DTO.Product
{
    public class ProductDTO
    {
        [Key] public int id { get; set; }
        public int? category_id { get; set; }
        [NotMapped] public string? category_name { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public decimal? price { get; set; }
        public string? img_url { get; set; }
        public int? stock { get; set; }
        public int? sold { get; set; }
    }
}
