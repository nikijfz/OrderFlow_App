using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using System.Net;

namespace OrderFlow_App.Frameworks.ResponseFrameworks
{
    public class Response<T> : IResponse<T>
    {

        public Response()
        {

        }

        public Response(bool isSuccessful, HttpStatusCode status, string message, T value)
        {
            IsSuccessful = isSuccessful;
            Status = status;
            Message = message;
            Value = value;
        }

        public bool IsSuccessful { get; set; }
        public HttpStatusCode Status { get; set; }
        public string? Message { get; set; }
        public T? Value { get; set; }

    }
}
