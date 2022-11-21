using Microsoft.AspNetCore.Mvc.Formatters;
using OrderReceiver.Helpers;

namespace OrderReceiver.InputFormatters
{
    public sealed class CsvStringInputFormatter : InputFormatter
    {
        public CsvStringInputFormatter()
        {
            //Which Content-Types this InputFormatter can handle
            SupportedMediaTypes.Add(AdditionalMediaTypeNames.TextCsv);
        }
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                var content = await reader.ReadToEndAsync();
                return InputFormatterResult.Success(content);
            }
        }
        protected override bool CanReadType(Type type)
        {
            //Which action parameter types this InputFormatter can handle
            return type == typeof(string);
        }
    }
}
