using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace YK.Application.Common.Exceptions
{
    public sealed class BusinessRuleException : AppException
    {
        public IReadOnlyCollection<string> Errors { get; }

        public BusinessRuleException(string message) : this([message])
        {
        }

        public BusinessRuleException(IEnumerable<string> errors)
            : base(
                HttpStatusCode.UnprocessableEntity,
                "business-rule-error",
                "Business Rule Validation Failed",
                "One or more business rules were violated.")
        {
            Errors = errors.ToArray();
        }
    }
}
