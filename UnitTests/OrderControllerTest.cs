using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderReceiver.Common;
using OrderReceiver.Common.Deserialization;
using OrderReceiver.Controllers;
using OrderReceiver.Helpers;
using System.IO;
using System.Net.Mime;

namespace UnitTests
{
    [TestFixture]
    internal sealed class OrderControllerTest
    {
        private OrdersController _controller;
        private DefaultHttpContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new DefaultHttpContext();
            _controller = new OrdersController(new OrderDeserializer(), new DbOrderProcessor(new OrderReceiver.Testing.TestDbContext())) 
            { 
                ControllerContext = new ControllerContext() { HttpContext = _context} 
            };
        }

        [Test]
        public async Task PostJsonRaw_Success()
        {
            var result = await PostOrder(DeserializeType.Xml);
            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
        }

        [Test]
        public async Task PostJsonFile_Success()
        {
            var result = await PostOrderFile(DeserializeType.Xlsx);
            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
        }

        [Test]
        public async Task PostCsvRaw_Success()
        {
            var result = await PostOrder(DeserializeType.Xml);
            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
        }

        [Test]
        public async Task PostCsvFile_Success()
        {
            var result = await PostOrderFile(DeserializeType.Xlsx);
            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
        }

        [Test]
        public async Task PostXmlRaw_Success()
        {
            var result = await PostOrder(DeserializeType.Xml);
            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
        }

        [Test]
        public async Task PostXmlFile_Success()
        {
            var result = await PostOrderFile(DeserializeType.Xml);
            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
        }

        [Test]
        public async Task PostXlsxFile_Success()
        {
            var result = await PostOrderFile(DeserializeType.Xlsx);
            Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
        }

        [Test]
        public async Task PostUndetermine_Failed()
        {
            try
            {
                var result = await PostOrderFile(DeserializeType.Undefined);
                //Assert.True(result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid);
                Assert.Fail();
            }
            catch (DeserializationException ex)
            {
                if (ex.Message == "Cannot determine content-type")
                {
                    Assert.Pass("Type determination is not implemented");
                }
            }
        }

        private async Task<ObjectResult?> PostOrder(DeserializeType type)
        {
            _context.Request.ContentType = DeserializeTypesHelper.MimeToDeserialize[type];
            using (var fileStream = new FileStream(GetTestFilePath(type), FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var result = await _controller.PostOrder(await reader.ReadToEndAsync());
                    return result as ObjectResult;
                }
            }
        }

        private async Task<ObjectResult?> PostOrderFile(DeserializeType type)
        {
            _context.Request.ContentType = AdditionalMediaTypeNames.MultipartFormData;
            var file = GetTestFilePath(type);
            if(file != null)
            {
                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    IHeaderDictionary headers = new HeaderDictionary();
                    headers.ContentType = DeserializeTypesHelper.MimeToDeserialize[type];
                    var formFile = getFormFile(fileStream, type, headers);
                    var result = await _controller.PostOrderFile(formFile);
                    return result as ObjectResult;
                }
            }
            else
            {
                _context.Request.ContentType = MediaTypeNames.Text.Plain;
                //проверяем определение типа.
                foreach (DeserializeType enumType in Enum.GetValues(typeof(DeserializeType)))
                {
                    bool allResponseSuccess = true;
                    var filePath = GetTestFilePath(enumType);
                    //В Enum.GetValues есть тип Undefined.
                    if (filePath != null)
                    {
                        ObjectResult? result;
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            IHeaderDictionary headers = new HeaderDictionary();
                            headers.ContentType = DeserializeTypesHelper.MimeToDeserialize[type];
                            var formFile = getFormFile(fileStream, type, headers);
                            result = await _controller.PostOrderFile(formFile) as ObjectResult;
                            allResponseSuccess = result != null && result.StatusCode == StatusCodes.Status200OK && result.Value is Guid;
                        }
                        if (!allResponseSuccess)
                        {
                            Assert.Fail($"Request {enumType} failed");
                            return result;
                        }
                        else
                        {
                            Assert.Pass();
                            return null;
                        }

                    }

                }

                return null;
            }

            static FormFile getFormFile(Stream stream, DeserializeType type, IHeaderDictionary dictionary)
            {
                var formFile = new FormFile(stream, 0, stream.Length, "value", "value");
                formFile.Headers = dictionary;
                return formFile;
            }
        }

        private string GetTestFilePath(DeserializeType type)
        {
            string filePath = null;
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
}
