namespace Aptiverse.Core.Exceptions
{
    public class PasswordResetException : Exception
    {
        public PasswordResetException(string message) : base(message) { }
        public PasswordResetException(string message, Exception innerException) : base(message, innerException) { }
    }
}
