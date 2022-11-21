using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace OrderReceiver.Models
{
    [PrimaryKey(nameof(Id))]
    public class ProductPack
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [XmlElement()]
        public Guid ProductId { get; set; }

        [XmlElement()]
        public int Amount { get; set; }
    }
}
