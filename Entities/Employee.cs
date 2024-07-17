using System.ComponentModel.DataAnnotations;

namespace SQL.Entities
{
    public class Employee
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public int positionid { get; set; }
        public decimal salary {  get; set; }
        public string email {  get; set; }
    }
}
