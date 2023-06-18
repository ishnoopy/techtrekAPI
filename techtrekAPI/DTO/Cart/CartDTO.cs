using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace techtrekAPI.DTO.Cart
{
    public class CartDTO
    {
        [Key] public int id { get; set; }

        public int? user_id { get; set; }

        public int? product_id { get; set; }

        [NotMapped] public string? name { get; set; }
        [NotMapped]  public decimal? price { get; set; }
        [NotMapped]  public string? img_url { get; set; }

        public int? qty { get; set; }
    }
}
