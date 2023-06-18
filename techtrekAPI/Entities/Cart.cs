using System.ComponentModel.DataAnnotations;

namespace techtrekAPI.Entities
{
    public class Cart
    {
        [Key] public int id { get; set; }
        public int? user_id { get; set; }
        public int? product_id { get; set; }
        public int? qty { get; set; }
    }
}
