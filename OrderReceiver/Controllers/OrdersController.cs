using Microsoft.AspNetCore.Mvc;
using OrderReceiver.Common.Deserialization;
using OrderReceiver.Helpers;
using OrderReceiver.Models;
using System.Net.Mime;

namespace OrderReceiver.Controllers
{
    /// <summary>
    /// Принимает Web Api запросы на заказы. Задача - десериализация и отправка дальше.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public sealed class OrdersController : ControllerBase
    {
        IOrderDeserializer deserializer;
        IOrderProcessor processor;

        public OrdersController(IOrderDeserializer deserializer, IOrderProcessor processor)
        {
            this.deserializer = deserializer;
            this.processor = processor;
        }

        //TODO: Было бы логично каким-то образом сразу получить Stream или IFormFile, но у меня не получилось.
        [HttpPost("PostOrder")]
        [Produces(typeof(Response<Guid>))]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml, MediaTypeNames.Text.Plain, AdditionalMediaTypeNames.TextCsv)]
        public async Task<IActionResult> PostOrder([FromBody] string body)
        {
            try
            {
                if(body == null)
                {
                    return BadRequest("Body cannot be empty");
                }
                var contentType = this.Request.ContentType;
                if (contentType == AdditionalMediaTypeNames.MultipartFormData)
                {
                    return BadRequest("For files use PostOrderFile instead");
                }
                else
                {
                    var type = IOrderDeserializer.GetEnumFromMimeType(contentType);

                    var result = await deserializer.DeserializeOrder(body.ToString(), type);
                    await processor.SendToProcess(result);
                    return Ok(result.ID);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("PostOrderFile")]
        [Produces(typeof(Response<Guid>))]
        public async Task<IActionResult> PostOrderFile(IFormFile value)
        {
            try
            {
                var contentType = value.ContentType;
                var type = IOrderDeserializer.GetEnumFromMimeType(contentType);
                var result = await deserializer.DeserializeOrder(value.OpenReadStream(), type);
                await processor.SendToProcess(result);
                return Ok(result.ID);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
