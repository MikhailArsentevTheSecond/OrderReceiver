using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace OrderReceiver.Models
{
    [PrimaryKey(nameof(ID))]
    public class Order
    {
        public Order()
        {
            ID = Guid.NewGuid();
            UserID = Guid.NewGuid();
            ProductPacks = new List<ProductPack>();
        }

        public Guid ID { get; }

        //Взять какой-то ID из системы авторизации
        public Guid UserID { get; }

        [XmlArray()]
        public List<ProductPack> ProductPacks { get; }
    }
}
