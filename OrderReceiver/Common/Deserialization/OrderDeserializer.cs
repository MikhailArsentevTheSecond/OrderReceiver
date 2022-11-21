using ClosedXML.Excel;
using Newtonsoft.Json;
using OrderReceiver.Models;
using System.Text;
using System.Xml.Serialization;

namespace OrderReceiver.Common.Deserialization
{
    public sealed class OrderDeserializer : IOrderDeserializer
    {
        public OrderDeserializer()
        {
        }

        public Task<Order> DeserializeOrder(string? body, DeserializeType type = DeserializeType.Undefined)
        {
            return DeserializeOrder(new MemoryStream(body == null ? Encoding.Default.GetBytes(string.Empty) : Encoding.Default.GetBytes(body)), type);
        }

        public Task<Order> DeserializeOrder(Stream body, DeserializeType type = DeserializeType.Undefined)
        {
            using (body)
            {
                if (type == DeserializeType.Undefined)
                {
                    type = DetermineType(body);
                }

                return type switch
                {
                    DeserializeType.Xlsx => GetXlsx(body),
                    DeserializeType.Xml => GetXml(body),
                    DeserializeType.Csv => GetCsv(body),
                    DeserializeType.Json => GetJson(body),
                    _ => throw new Exception($"Can't deserialize content of type {type}"),
                };
            }
        }

        //Пока что не уверен как лучше сделать этот метод. 
        private DeserializeType DetermineType(Stream body)
        {
            throw new DeserializationException("Cannot determine content-type");
        }

        private Task<Order> GetXlsx(Stream body)
        {

            using (var document = new XLWorkbook(body))
            {
                //Гипотетически можно отправлять несколько заказов.
                var worksheet = document.Worksheets.FirstOrDefault();
                if(worksheet == null)
                {
                    throw new DeserializationException("Worksheet not found");
                }

                var order = new Order();


                foreach (var row in worksheet.RowsUsed())
                {
                    var cellCount = row.CellCount();
                    if (cellCount < 2)
                    {
                        throw new DeserializationException($"Wrong worksheet format. Column count {cellCount} expected at least 2");
                    }
                    //избегаем возможного заголовка
                    if (row.RowNumber() == 1)
                    {
                        if (!Guid.TryParse(row.Cell(1).Value?.ToString(), out _))
                        {
                            continue;
                        }
                    }
                    var productIdCell = row.Cell(1);
                    var amountCell = row.Cell(2);
                    if (!Guid.TryParse(productIdCell.Value.ToString(), out Guid productIdBuffer))
                    {
                        throw new DeserializationException($"Cannot parse {nameof(ProductPack.ProductId)} in {productIdCell.Address}");
                    }
                    if (!int.TryParse(amountCell.Value.ToString(), out int amountBuffer))
                    {
                        throw new DeserializationException($"Cannot parse {nameof(ProductPack.Amount)} in {amountCell.Address}");
                    }

                    var pack = new ProductPack() { Id = Guid.NewGuid(), Amount = amountBuffer, ProductId = productIdBuffer};
                    order.ProductPacks.Add(pack);
                }
                return Task.FromResult(order);
            }
        }

        private async Task<Order> GetCsv(Stream body)
        {
            var result = new Order();
            using (var streamReader = new StreamReader(body))
            {
                int lineNumber = 1;
                while (!streamReader.EndOfStream)
                {
                    var rawProductPack = await streamReader.ReadLineAsync();
                    if(rawProductPack != null)
                    {
                        var splitValues = rawProductPack.Split(',');
                        if(splitValues.Length < 2)
                        {
                            throw new DeserializationException($"Wrong csv format. Column count {splitValues.Length} expected at least 2");
                        }
                        if (lineNumber == 1)
                        {
                            //Проверить является ли первая строчка заголовком
                            if (!Guid.TryParse(splitValues[0], out _))
                            {
                                continue;
                            }
                        }
                        if (!Guid.TryParse(splitValues[0], out Guid productIdBuffer))
                        {
                            throw new DeserializationException($"Cannot parse {nameof(ProductPack.ProductId)} in {lineNumber}");
                        }
                        if (!int.TryParse(splitValues[1], out int amountBuffer))
                        {
                            throw new DeserializationException($"Cannot parse {nameof(ProductPack.Amount)} in {lineNumber}");
                        }

                        result.ProductPacks.Add(new ProductPack() { ProductId = productIdBuffer, Amount = amountBuffer});
                    }
                    lineNumber++;
                }
            }
            return result;
        }

        private Task<Order> GetXml(Stream body)
        {
            var xml = new XmlSerializer(typeof(Order));
            var result = xml.Deserialize(body) as Order;

            return Task.FromResult(result);
        }

        private async Task<Order> GetJson(Stream body)
        {
            using (var streamReader = new StreamReader(body))
            {
                var content = await streamReader.ReadToEndAsync();
                var result = JsonConvert.DeserializeObject<Order>(content);
                return result;
            }
        }
    }
}
