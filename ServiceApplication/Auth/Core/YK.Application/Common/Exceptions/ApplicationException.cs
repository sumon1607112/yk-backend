using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace YK.Application.Common.Exceptions
{
    public abstract class AppException: Exception
    {
        public HttpStatusCode StatusCode { get; }

        public string Type { get; }

        public string Title { get; }

        protected AppException(HttpStatusCode statusCode,string type,string title,string message) : base(message)
        {
            StatusCode = statusCode;
            Type = type;
            Title = title;
        }
    }
}
