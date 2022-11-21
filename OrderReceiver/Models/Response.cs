using System.Net;

namespace OrderReceiver.Models
{
    public class Response
    {
        public string ErrorMessage { get; set; }
    }

    public class Response<T> : Response
    {
        public Response(T result)
        {
            Result = result;
        }

        public T Result { get; set; }
    }
}
