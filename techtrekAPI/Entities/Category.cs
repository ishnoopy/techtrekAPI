using System.ComponentModel.DataAnnotations;

namespace techtrekAPI.Entities
{
    public class Category
    {
        [Key] public int id { get; set; }
        [Required] public string name { get; set; }
    }
}
