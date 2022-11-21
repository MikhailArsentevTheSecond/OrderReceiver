using Microsoft.EntityFrameworkCore;

namespace OrderReceiver.Models
{
    [PrimaryKey(nameof(Id))]
    public class Product
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public float Cost { get; set; }

    }
}
