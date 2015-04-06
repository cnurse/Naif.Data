using Naif.Data.ComponentModel;

namespace Naif.TestUtilities.Models
{
    [PrimaryKey(KeyField = "ID")]
    public class Dog
    {
        public int? Age { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
