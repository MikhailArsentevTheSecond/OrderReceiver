using OrderReceiver.Common;
using OrderReceiver.Common.Deserialization;
using OrderReceiver.Models;
using System.Runtime.CompilerServices;

namespace OrderReceiver.Tests
{
    [TestFixture]
    public class OrderDeserializerTest
    {
        private OrderDeserializer _deserializer;
        private Order _checkObj;

        [SetUp]
        public void SetUp()
        {
            _deserializer = new OrderDeserializer();
            //TODO: Генерировать файлы.
            var order = new Order();
            order.ProductPacks.AddRange(new List<ProductPack>()
                {
                    new ProductPack() { Amount = 10, Id = Guid.NewGuid(), ProductId = Guid.Parse("3ee64405-f377-4394-a010-5b87acfb938e")},
                    new ProductPack() { Amount = 50, Id = Guid.NewGuid(), ProductId = Guid.Parse("bf511429-a461-48de-8e9d-d16e8206f802")},
                    new ProductPack() { Amount = 150, Id = Guid.NewGuid(), ProductId = Guid.Parse("eab47b14-c1b1-4396-ba1d-5ae03e25b8da")}
                });
            _checkObj = order;
        }

        [Test]
        public async Task Json_Success()
        {
            await CheckDeserialization(DeserializeType.Json);
        }

        [Test]
        public async Task Xml_Success()
        {
            await CheckDeserialization(DeserializeType.Xml);
        }

        //Очень долго выполняется тест. В идеале надо делать xlsx документ в памяти (возможно. OpenXml тоже мог бы ускорить).
        [Test]
        public async Task Xlsx_Success()
        {
            await CheckDeserialization(DeserializeType.Xlsx);
        }

        [Test]
        public async Task Csv_Success()
        {
            await CheckDeserialization(DeserializeType.Csv);
        }

        [Test]
        public void DetermineType_Success()
        {
            Assert.True(true);
        }

        private async Task CheckDeserialization(DeserializeType type)
        {
            var filePath = getFilePath(type);
            if (filePath == null)
            {
                bool allDeserializedCorrectly = false;
                //Для проверки определения типа необходимо пройтись по всем файлам.
                foreach (DeserializeType enumType in Enum.GetValues(typeof(DeserializeType)))
                {
                    filePath = getFilePath(enumType);
                    //В Enum.GetValues есть тип Undefined.
                    if(filePath != null)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var deserialized = await _deserializer.DeserializeOrder(stream);
                            allDeserializedCorrectly = Compare(_checkObj, deserialized);
                        }
                    }
                }
                Assert.True(allDeserializedCorrectly);
            }
            else
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var deserialized = await _deserializer.DeserializeOrder(stream, type);
                    Assert.True(Compare(_checkObj, deserialized));
                }
            }

            static string? getFilePath(DeserializeType type)
            {
                string? filePath = null;
                switch (type)
                {
                    case DeserializeType.Xlsx:
                        filePath = "TestFiles\\xlsx_success.xlsx";
                        break;
                    case DeserializeType.Xml:
                        filePath = "TestFiles\\xml_success.xml";
                        break;
                    case DeserializeType.Csv:
                        filePath = "TestFiles\\csv_success.csv";
                        break;
                    case DeserializeType.Json:
                        filePath = "TestFiles\\json_success.json";
                        break;
                    case DeserializeType.Undefined:
                        break;
                    default:
                        Assert.Fail($"No AutoTest for type {type}");
                        break;
                }

                return filePath;
            }
        }

        private bool Compare(Order inMemoryVersion, Order deserialized)
        {
            if(deserialized == null)
            {
                throw new DeserializationException("Product is null");
            }
            if(deserialized.ProductPacks == null || deserialized.ProductPacks.Count == 0)
            {
                throw new DeserializationException("ProductPack is null or empty");
            }

            if(inMemoryVersion.ProductPacks.Count == deserialized.ProductPacks.Count)
            {
                var productPackLeft = inMemoryVersion.ProductPacks;
                var productPackRight = deserialized.ProductPacks;

                for (int i = 0; i < productPackLeft.Count; i++)
                {
                    if(productPackLeft[i].Amount != productPackRight[i].Amount || productPackLeft[i].ProductId != productPackRight[i].ProductId)
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new DeserializationException($"ProductPack parse Exception. Expected {inMemoryVersion.ProductPacks.Count} got {deserialized.ProductPacks.Count}");
            }

            return true;
        }
    }
}
