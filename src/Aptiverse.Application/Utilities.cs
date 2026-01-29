namespace Aptiverse.Application
{
    public static class Utilities
    {
        public static string GenerateEmailConfirmationTemplate(string firstName, string lastName, string? userName, string? email, string userType, string confirmationLink)
        {
            return $@"
            <!DOCTYPE html>
            <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Confirm Your Email - Aptiverse</title>
                    <style>
                        /* Your existing CSS styles here */
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                        }}
                        /* ... include all your existing styles ... */
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Welcome to Aptiverse! 🎉</h1>
                        </div>

                        <div class='content'>
                            <p class='welcome-text'>Hello <strong>{firstName} {lastName}</strong>,</p>
                            <p>Thank you for joining Aptiverse! We're excited to have you on board. To complete your registration and start using your account, please confirm your email address.</p>
        
                            <div class='user-info'>
                                <strong>Account Details:</strong><br>
                                • Username: <strong>{userName}</strong><br>
                                • Email: <strong>{email}</strong><br>
                                • Account Type: <strong>{userType}</strong>
                            </div>

                            <div class='steps'>
                                <div class='step'>
                                    <div class='step-number'>1</div>
                                    <div>Click the confirmation button below</div>
                                </div>
                                <div class='step'>
                                    <div class='step-number'>2</div>
                                    <div>You'll be redirected to verify your email</div>
                                </div>
                                <div class='step'>
                                    <div class='step-number'>3</div>
                                    <div>Start exploring Aptiverse features!</div>
                                </div>
                            </div>

                            <div style='text-align: center;'>
                                <a href='{confirmationLink}' class='confirmation-button'>
                                    Confirm My Email Address
                                </a>
                            </div>

                            <p class='alternative-text'>If the button doesn't work, copy and paste this URL into your browser:</p>
                            <div class='confirmation-link'>{confirmationLink}</div>

                            <div class='support-info'>
                                <p><strong>Need help?</strong><br>
                                If you didn't create this account or need assistance, please contact our support team immediately.</p>
                                <p>This confirmation link will expire in 24 hours for security reasons.</p>
                            </div>
                        </div>

                        <div class='footer'>
                            <p>&copy; {DateTime.UtcNow.Year} Aptiverse. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
            </html>";
        }
    }
}