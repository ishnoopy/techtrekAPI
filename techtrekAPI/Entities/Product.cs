using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace techtrekAPI.Entities
{
    public class Product
    {
        [Key] public int id { get; set; }
        [Required] public int category_id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }

        public decimal? price { get; set; }
        [NotMapped] public IFormFile? img { get; set; }
        public string? img_url { get; set; }
        public int? stock { get; set; }
        public int? sold { get; set; }
    }
}
