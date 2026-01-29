namespace Aptiverse.Core.Exceptions
{
    public class UserNotFoundException : PasswordResetException
    {
        public UserNotFoundException() : base("Invalid reset request") { }
    }
}
