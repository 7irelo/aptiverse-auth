namespace Aptiverse.Core.Exceptions
{
    public class PasswordChangeException : Exception
    {
        public PasswordChangeException(string message) : base(message) { }
        public PasswordChangeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
