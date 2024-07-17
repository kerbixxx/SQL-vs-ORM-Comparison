using System.ComponentModel.DataAnnotations;

namespace SQL.Entities
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        public string Name {  get; set; }
    }
}
