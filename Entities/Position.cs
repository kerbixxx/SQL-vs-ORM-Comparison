using System.ComponentModel.DataAnnotations;

namespace SQL.Entities
{
    public class Position
    {
        [Key]
        public int id { get; set; }
        public string name {  get; set; }
    }
}
