namespace OutReachToursAPI.Services
{
    public static class EmailTemplates
    {
        private const string PrimaryGold = "#C5A572";
        private const string DarkBg = "#0A0B0D";
        private const string CardBg = "#131517";
        private const string BorderColor = "#1E2023";
        private const string TextMuted = "#8A8F98";
        private const string TextWhite = "#FFFFFF";

        private static string WrapInLayout(string bodyContent, string preheader = "")
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <title>Outreach Tours</title>
    <!--[if mso]>
    <noscript>
        <xml>
            <o:OfficeDocumentSettings>
                <o:PixelsPerInch>96</o:PixelsPerInch>
            </o:OfficeDocumentSettings>
        </xml>
    </noscript>
    <![endif]-->
</head>
<body style=""margin:0;padding:0;background-color:{DarkBg};font-family:'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;-webkit-font-smoothing:antialiased;"">
    {(string.IsNullOrEmpty(preheader) ? "" : $@"<div style=""display:none;font-size:1px;color:{DarkBg};line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden;"">{preheader}</div>")}
    
    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color:{DarkBg};"">
        <tr>
            <td align=""center"" style=""padding:40px 20px;"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""max-width:600px;width:100%;"">
                    
                    <!-- Logo Header -->
                    <tr>
                        <td align=""center"" style=""padding-bottom:32px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td style=""background-color:#FFFFFF;border-radius:12px;padding:12px 24px;"">
                                        <span style=""font-size:22px;font-weight:700;color:{DarkBg};letter-spacing:-0.5px;"">
                                            <span style=""color:{PrimaryGold};font-style:italic;"">O</span>utreach
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Main Card -->
                    <tr>
                        <td style=""background-color:{CardBg};border:1px solid {BorderColor};border-radius:16px;overflow:hidden;"">
                            
                            <!-- Gold accent bar -->
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""height:4px;background:linear-gradient(90deg, {PrimaryGold}, #D4B87A, {PrimaryGold});""></td>
                                </tr>
                            </table>
                            
                            <!-- Body Content -->
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""padding:40px 40px 32px 40px;"">
                                        {bodyContent}
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding:24px 0;text-align:center;"">
                            <p style=""margin:0 0 8px 0;font-size:12px;color:{TextMuted};"">
                                Outreach Tours &mdash; Curating Extraordinary African Journeys
                            </p>
                            <p style=""margin:0;font-size:11px;color:#555;"">
                                &copy; {DateTime.Now.Year} Outreach Tours. All rights reserved.
                            </p>
                        </td>
                    </tr>
                    
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        /// <summary>
        /// Branded invite email sent when a new user is added to the system.
        /// Contains a password reset link for them to set their initial password.
        /// </summary>
        public static (string plain, string html) GetInviteEmail(string userName, string resetUrl)
        {
            var plain = $@"Welcome to Outreach Tours, {userName}!

You've been invited to join the Outreach Tours CRM platform. 

To get started, please set your password by visiting the link below:
{resetUrl}

This link will expire in 72 hours.

If you didn't expect this invitation, you can safely ignore this email.

— The Outreach Tours Team";

            var body = $@"
                <h1 style=""margin:0 0 8px 0;font-size:24px;font-weight:700;color:{TextWhite};"">Welcome aboard! 🎉</h1>
                <p style=""margin:0 0 24px 0;font-size:14px;color:{TextMuted};line-height:1.5;"">
                    Hi {EscapeHtml(userName)}, you've been invited to join the <strong style=""color:{TextWhite};"">Outreach Tours CRM</strong> platform.
                </p>
                
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;"">
                    <tr>
                        <td style=""background-color:rgba(197,165,114,0.08);border:1px solid rgba(197,165,114,0.15);border-radius:12px;padding:20px;"">
                            <p style=""margin:0 0 4px 0;font-size:12px;color:{PrimaryGold};text-transform:uppercase;letter-spacing:1px;font-weight:600;"">Next Step</p>
                            <p style=""margin:0;font-size:14px;color:{TextWhite};line-height:1.5;"">
                                Set your password to activate your account and start managing tours, clients, and more.
                            </p>
                        </td>
                    </tr>
                </table>
                
                <!-- CTA Button -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;"">
                    <tr>
                        <td align=""center"">
                            <a href=""{EscapeHtml(resetUrl)}"" target=""_blank"" style=""display:inline-block;background-color:{PrimaryGold};color:{DarkBg};font-size:14px;font-weight:700;text-decoration:none;padding:14px 40px;border-radius:8px;letter-spacing:0.3px;"">
                                Set Your Password
                            </a>
                        </td>
                    </tr>
                </table>
                
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-top:1px solid {BorderColor};padding-top:16px;"">
                    <tr>
                        <td>
                            <p style=""margin:0 0 8px 0;font-size:12px;color:{TextMuted};line-height:1.6;"">
                                🔗 Can't click the button? Copy and paste this link into your browser:
                            </p>
                            <p style=""margin:0;font-size:11px;color:{PrimaryGold};word-break:break-all;line-height:1.6;"">
                                {EscapeHtml(resetUrl)}
                            </p>
                            <p style=""margin:12px 0 0 0;font-size:11px;color:#555;"">
                                This link expires in 72 hours.
                            </p>
                        </td>
                    </tr>
                </table>";

            var html = WrapInLayout(body, $"Welcome to Outreach Tours, {userName}! Set your password to get started.");
            return (plain, html);
        }

        /// <summary>
        /// Branded invoice email sent when a POS transaction is created.
        /// Contains the invoice details and a Paystack payment link.
        /// </summary>
        public static (string plain, string html) GetInvoiceEmail(string clientName, string invoiceNumber, double amountKES, string paymentUrl, string? segment)
        {
            var formattedAmount = amountKES.ToString("N0");
            var segmentLabel = segment ?? "Journey";

            var plain = $@"Hello {clientName},

Your invoice {invoiceNumber} for KES {formattedAmount} has been generated.

Pay securely here: {paymentUrl}

Thank you for choosing Outreach Tours.

— The Outreach Tours Team";

            var body = $@"
                <h1 style=""margin:0 0 8px 0;font-size:24px;font-weight:700;color:{TextWhite};"">Invoice Ready</h1>
                <p style=""margin:0 0 24px 0;font-size:14px;color:{TextMuted};line-height:1.5;"">
                    Hello {EscapeHtml(clientName)}, your invoice is ready for payment.
                </p>
                
                <!-- Invoice Details Card -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(255,255,255,0.03);border:1px solid {BorderColor};border-radius:12px;overflow:hidden;"">
                    <tr>
                        <td style=""padding:20px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""padding-bottom:12px;border-bottom:1px solid {BorderColor};"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Invoice Number</p>
                                        <p style=""margin:4px 0 0 0;font-size:16px;font-weight:700;color:{TextWhite};"">{EscapeHtml(invoiceNumber)}</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Segment</p>
                                        <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};"">{EscapeHtml(segmentLabel)}</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding-top:12px;"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Amount Due</p>
                                        <p style=""margin:4px 0 0 0;font-size:28px;font-weight:700;color:{PrimaryGold};"">KES {formattedAmount}</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                
                <!-- Pay Button -->
                {(string.IsNullOrEmpty(paymentUrl) ? "" : $@"
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;"">
                    <tr>
                        <td align=""center"">
                            <a href=""{EscapeHtml(paymentUrl)}"" target=""_blank"" style=""display:inline-block;background-color:{PrimaryGold};color:{DarkBg};font-size:14px;font-weight:700;text-decoration:none;padding:14px 40px;border-radius:8px;letter-spacing:0.3px;"">
                                Pay Securely Now
                            </a>
                        </td>
                    </tr>
                </table>
                
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-top:1px solid {BorderColor};padding-top:16px;"">
                    <tr>
                        <td>
                            <p style=""margin:0 0 8px 0;font-size:12px;color:{TextMuted};line-height:1.6;"">
                                🔗 Payment link:
                            </p>
                            <p style=""margin:0;font-size:11px;color:{PrimaryGold};word-break:break-all;line-height:1.6;"">
                                {EscapeHtml(paymentUrl)}
                            </p>
                        </td>
                    </tr>
                </table>")}
                
                <p style=""margin:24px 0 0 0;font-size:13px;color:{TextMuted};line-height:1.6;"">
                    Thank you for choosing Outreach Tours. If you have any questions about this invoice, please don't hesitate to reach out.
                </p>";

            var html = WrapInLayout(body, $"Invoice {invoiceNumber} for KES {formattedAmount} is ready for payment.");
            return (plain, html);
        }

        /// <summary>
        /// Branded email sent when a user requests a password reset.
        /// </summary>
        public static (string plain, string html) GetForgotPasswordEmail(string userName, string resetUrl)
        {
            var plain = $@"Hello {userName},

We received a request to reset your password for your Outreach Tours account.

To reset your password, please click the link below:
{resetUrl}

This link will expire in 2 hours.

If you did not request a password reset, please ignore this email or contact support if you have concerns.

— The Outreach Tours Team";

            var body = $@"
                <h1 style=""margin:0 0 8px 0;font-size:24px;font-weight:700;color:{TextWhite};"">Password Reset Request</h1>
                <p style=""margin:0 0 24px 0;font-size:14px;color:{TextMuted};line-height:1.5;"">
                    Hi {EscapeHtml(userName)}, we received a request to reset the password for your <strong style=""color:{TextWhite};"">Outreach Tours CRM</strong> account.
                </p>
                
                <!-- CTA Button -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;"">
                    <tr>
                        <td align=""center"">
                            <a href=""{EscapeHtml(resetUrl)}"" target=""_blank"" style=""display:inline-block;background-color:{PrimaryGold};color:{DarkBg};font-size:14px;font-weight:700;text-decoration:none;padding:14px 40px;border-radius:8px;letter-spacing:0.3px;"">
                                Reset Password
                            </a>
                        </td>
                    </tr>
                </table>
                
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-top:1px solid {BorderColor};padding-top:16px;"">
                    <tr>
                        <td>
                            <p style=""margin:0 0 8px 0;font-size:12px;color:{TextMuted};line-height:1.6;"">
                                🔗 Can't click the button? Copy and paste this link into your browser:
                            </p>
                            <p style=""margin:0;font-size:11px;color:{PrimaryGold};word-break:break-all;line-height:1.6;"">
                                {EscapeHtml(resetUrl)}
                            </p>
                            <p style=""margin:12px 0 0 0;font-size:11px;color:#555;"">
                                This link expires in 2 hours. If you did not request a password reset, you can safely ignore this email.
                            </p>
                        </td>
                    </tr>
                </table>";

            var html = WrapInLayout(body, $"Reset your Outreach Tours password");
            return (plain, html);
        }

        private static string EscapeHtml(string input)
        {
            return System.Net.WebUtility.HtmlEncode(input ?? "");
        }
    }
}
