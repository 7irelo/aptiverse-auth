namespace Aptiverse.Core.Exceptions
{
    public class InvalidResetTokenException : PasswordResetException
    {
        public InvalidResetTokenException() : base("Invalid or expired reset token") { }
    }
}
