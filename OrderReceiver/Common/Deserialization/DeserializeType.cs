using OrderReceiver.Helpers;
using System.Net.Mime;

namespace OrderReceiver.Common.Deserialization
{
    public enum DeserializeType
    {
        Undefined,
        Xlsx,
        Xml,
        Csv,
        Json
    }

    public static class DeserializeTypesHelper
    {
        public static readonly IReadOnlyDictionary<DeserializeType, string> MimeToDeserialize = new Dictionary<DeserializeType, string>()
        {
            { DeserializeType.Undefined, MediaTypeNames.Text.Plain  },
            { DeserializeType.Xlsx, AdditionalMediaTypeNames.ApplicationXlsx},
            { DeserializeType.Xml, MediaTypeNames.Application.Xml   },
            { DeserializeType.Csv, AdditionalMediaTypeNames.TextCsv },
            { DeserializeType.Json, MediaTypeNames.Application.Json },
        };
    }
}
