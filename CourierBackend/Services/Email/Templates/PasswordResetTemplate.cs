namespace CourierBackend.Services.Email.Templates;

public static class PasswordResetTemplate
{
    public static string Build(string name, string resetUrl)
    {
        return $"""
            <html>
            <body>
                <h2>Password Reset Request</h2>

                <p>Hello {name},</p>

                <p>We received a request to reset your password.</p>

                <p>
                    <a href="{resetUrl}">
                        Reset Password
                    </a>
                </p>

                <p>
                    This link expires in 30 minutes.
                </p>

                <p>
                    If you didn't request this reset,
                    please ignore this email.
                </p>
            </body>
            </html>
            """;
    }
}