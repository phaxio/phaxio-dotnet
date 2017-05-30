using System;

namespace Phaxio.Errors.V2
{
    public class RateLimitException : Exception
    {
        public RateLimitException(string message) : base(message) { }
    }

    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message) { }
    }

    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ApiConnectionException : Exception
    {
        public ApiConnectionException(string message) : base(message) { }
    }

    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
    }
}
