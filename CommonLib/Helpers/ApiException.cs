using System;
using System.Net;

namespace CommonLib.Helpers
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }

        public ApiException(HttpStatusCode code, string mess)
        {
            StatusCode = code;
            Message = mess;
        }
    }
}
