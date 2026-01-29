namespace Aptiverse.Core.Exceptions
{
    public class TokenRefreshException : Exception
    {
        public TokenRefreshException(string message) : base(message) { }
        public TokenRefreshException(string message, Exception innerException) : base(message, innerException) { }
    }
}
