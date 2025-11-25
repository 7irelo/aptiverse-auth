namespace Aptiverse.Core.Exceptions
{
    public class AuthenticationException : Exception
    {
        public string ErrorCode { get; }

        public AuthenticationException() 
            : base()
        {
            ErrorCode = string.Empty;
        }

        public AuthenticationException(string message)
            : base(message)
        {
            ErrorCode = string.Empty;
        }

        public AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = string.Empty;
        }

        public AuthenticationException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public AuthenticationException(string errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}