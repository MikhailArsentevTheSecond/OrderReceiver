using OrderReceiver.Helpers;
using OrderReceiver.Models;
using System.Net.Mime;

namespace OrderReceiver.Common.Deserialization
{
    public interface IOrderDeserializer
    {
        public Task<Order> DeserializeOrder(Stream body, DeserializeType type = DeserializeType.Undefined);

        public Task<Order> DeserializeOrder(string? body, DeserializeType type = DeserializeType.Undefined);

        public static DeserializeType GetEnumFromMimeType(string? mimeType)
        {
            DeserializeType type;
            switch (mimeType)
            {
                case MediaTypeNames.Application.Json:
                    type = DeserializeType.Json;
                    break;
                case MediaTypeNames.Text.Xml:
                case MediaTypeNames.Application.Xml:
                    type = DeserializeType.Xml;
                    break;
                case AdditionalMediaTypeNames.TextCsv:
                    type = DeserializeType.Csv;
                    break;
                case AdditionalMediaTypeNames.ApplicationXlsx:
                    type = DeserializeType.Xlsx;
                    break;
                default:
                    type = DeserializeType.Undefined;
                    break;
            }

            return type;
        }
    }
}
