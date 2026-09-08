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

        /// <summary>
        /// Branded email sent to a client when a provisional hotel hold is placed.
        /// Informs them of the reservation details arranged under their trip package (no payment link).
        /// </summary>
        public static (string plain, string html) GetHoldNotificationEmail(
            string clientName, string hotelName, string roomType, string checkIn, string checkOut,
            double totalAmountKES, string voucherNumber)
        {
            var formattedAmount = totalAmountKES.ToString("N0");

            var plain = $@"Hello {clientName},

A provisional room reservation hold has been placed on your behalf by Outreach Tours as part of your booked trip itinerary.

Hotel: {hotelName}
Room: {roomType}
Check-in: {checkIn}
Check-out: {checkOut}
Accommodation Value: KES {formattedAmount}
Voucher Number: {voucherNumber}

This accommodation hold is arranged and coordinated directly under your Outreach Tours trip package. No separate payment is required from you for this hold. Our concierge team is coordinating all details for your stay.

If you have any special requests or questions regarding your itinerary, please feel free to reach out to us.

— The Outreach Tours Team";

            var body = $@"
                <h1 style=""margin:0 0 8px 0;font-size:24px;font-weight:700;color:{TextWhite};"">Room Hold Reserved 🏨</h1>
                <p style=""margin:0 0 24px 0;font-size:14px;color:{TextMuted};line-height:1.5;"">
                    Hello {EscapeHtml(clientName)}, a provisional room hold has been reserved for your upcoming journey with Outreach Tours.
                </p>
                
                <!-- Hotel Details Card -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(255,255,255,0.03);border:1px solid {BorderColor};border-radius:12px;overflow:hidden;"">
                    <tr>
                        <td style=""padding:20px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""padding-bottom:12px;border-bottom:1px solid {BorderColor};"">
                                        <p style=""margin:0;font-size:11px;color:{PrimaryGold};text-transform:uppercase;letter-spacing:1px;font-weight:600;"">Reserved Hotel</p>
                                        <p style=""margin:4px 0 0 0;font-size:16px;font-weight:700;color:{TextWhite};"">{EscapeHtml(hotelName)}</p>
                                        <p style=""margin:4px 0 0 0;font-size:12px;color:{TextMuted};"">{EscapeHtml(roomType)}</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                            <tr>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Check-in</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};font-weight:600;"">{EscapeHtml(checkIn)}</p>
                                                </td>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Check-out</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};font-weight:600;"">{EscapeHtml(checkOut)}</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Voucher Reference</p>
                                        <p style=""margin:4px 0 0 0;font-size:14px;font-weight:700;color:{PrimaryGold};"">{EscapeHtml(voucherNumber)}</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding-top:12px;"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Accommodation Value</p>
                                        <p style=""margin:4px 0 0 0;font-size:24px;font-weight:700;color:{PrimaryGold};"">KES {formattedAmount}</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                
                <!-- Trip Package Notice -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(217,119,6,0.08);border:1px solid rgba(217,119,6,0.2);border-radius:12px;"">
                    <tr>
                        <td style=""padding:16px 20px;"">
                            <p style=""margin:0;font-size:13px;color:#f59e0b;font-weight:600;"">📋 Arranged Under Your Trip Package</p>
                            <p style=""margin:6px 0 0 0;font-size:12px;color:{TextMuted};line-height:1.5;"">
                                This room hold is reserved as part of your scheduled itinerary with Outreach Tours. <strong style=""color:{TextWhite};"">No separate payment is required from you</strong>—it is already accounted for within your client trip package.
                            </p>
                        </td>
                    </tr>
                </table>
                
                <p style=""margin:24px 0 0 0;font-size:13px;color:{TextMuted};line-height:1.6;"">
                    Our team will finalize all arrival coordination with the hotel. If you have any questions or custom preferences, please don't hesitate to reach out.
                </p>";

            var html = WrapInLayout(body, $"Room hold at {hotelName} for {checkIn} to {checkOut} — Voucher {voucherNumber}");
            return (plain, html);
        }

        /// <summary>
        /// Branded email sent to a client when their hotel booking is confirmed (payment received).
        /// </summary>
        public static (string plain, string html) GetBookingConfirmedEmail(
            string clientName, string hotelName, string roomType, string checkIn, string checkOut,
            double totalAmountKES, string voucherNumber)
        {
            var formattedAmount = totalAmountKES.ToString("N0");

            var plain = $@"Hello {clientName},

Great news! Your hotel booking has been confirmed.

Hotel: {hotelName}
Room: {roomType}
Check-in: {checkIn}
Check-out: {checkOut}
Total Paid: KES {formattedAmount}
Voucher: {voucherNumber}

Your reservation is guaranteed. Please present your voucher number upon check-in.

— The Outreach Tours Team";

            var body = $@"
                <h1 style=""margin:0 0 8px 0;font-size:24px;font-weight:700;color:{TextWhite};"">Booking Confirmed ✅</h1>
                <p style=""margin:0 0 24px 0;font-size:14px;color:{TextMuted};line-height:1.5;"">
                    Hello {EscapeHtml(clientName)}, your hotel reservation is now <strong style=""color:#10b981;"">confirmed and guaranteed</strong>.
                </p>
                
                <!-- Booking Details Card -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(16,185,129,0.05);border:1px solid rgba(16,185,129,0.2);border-radius:12px;overflow:hidden;"">
                    <tr>
                        <td style=""padding:20px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""padding-bottom:12px;border-bottom:1px solid {BorderColor};"">
                                        <p style=""margin:0;font-size:11px;color:#10b981;text-transform:uppercase;letter-spacing:1px;font-weight:600;"">Confirmed Hotel</p>
                                        <p style=""margin:4px 0 0 0;font-size:16px;font-weight:700;color:{TextWhite};"">{EscapeHtml(hotelName)}</p>
                                        <p style=""margin:4px 0 0 0;font-size:12px;color:{TextMuted};"">{EscapeHtml(roomType)}</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                            <tr>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Check-in</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};font-weight:600;"">{EscapeHtml(checkIn)}</p>
                                                </td>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Check-out</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};font-weight:600;"">{EscapeHtml(checkOut)}</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Voucher Number</p>
                                        <p style=""margin:4px 0 0 0;font-size:18px;font-weight:700;color:{PrimaryGold};letter-spacing:1px;"">{EscapeHtml(voucherNumber)}</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding-top:12px;"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Amount Paid</p>
                                        <p style=""margin:4px 0 0 0;font-size:28px;font-weight:700;color:#10b981;"">KES {formattedAmount}</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(255,255,255,0.03);border:1px solid {BorderColor};border-radius:12px;"">
                    <tr>
                        <td style=""padding:16px 20px;"">
                            <p style=""margin:0;font-size:12px;color:{TextWhite};font-weight:600;"">📋 Check-in Instructions</p>
                            <p style=""margin:6px 0 0 0;font-size:12px;color:{TextMuted};line-height:1.5;"">
                                Please present your voucher number <strong style=""color:{PrimaryGold};"">{EscapeHtml(voucherNumber)}</strong> at the front desk upon arrival. Your room has been guaranteed for the dates above.
                            </p>
                        </td>
                    </tr>
                </table>
                
                <p style=""margin:0;font-size:13px;color:{TextMuted};line-height:1.6;"">
                    Thank you for choosing Outreach Tours. We hope you enjoy your stay!
                </p>";

            var html = WrapInLayout(body, $"Your booking at {hotelName} is confirmed — Voucher {voucherNumber}");
            return (plain, html);
        }

        /// <summary>
        /// Official agency booking order sent to the hotel's reservation desk.
        /// Guarantees the room under Outreach Tours corporate account.
        /// </summary>
        public static (string plain, string html) GetHotelDeskBookingOrderEmail(
            string hotelName, string voucherNumber, string clientName, int guestsCount, int roomsCount,
            string roomType, string checkIn, string checkOut, double totalAmountKES, string notes)
        {
            var formattedAmount = totalAmountKES.ToString("N0");

            var plain = $@"ATTN: RESERVATION DESK — {hotelName}

OFFICIAL AGENCY ROOM BOOKING ORDER / VOUCHER

Voucher / Booking Ref: {voucherNumber}
Agency: Outreach Tours & Safaris Ltd (Corporate Account)

Guest Name: {clientName}
Number of Guests: {guestsCount}
Number of Rooms: {roomsCount}
Room Category: {roomType}
Check-In Date: {checkIn}
Check-Out Date: {checkOut}
Agreed Total Value: KES {formattedAmount}

Guest Special Requests / Notes:
{(string.IsNullOrWhiteSpace(notes) ? "None specified" : notes)}

BILLING INSTRUCTIONS:
This booking is guaranteed by Outreach Tours. Please bill all room and included meal plan charges to our corporate account. Guest will present voucher reference {voucherNumber} upon arrival.

Please reply to this email to acknowledge receipt and confirm your internal reservation number.

Warm regards,
Outreach Tours Operations & Concierge Desk
Email: operations@outreachtours.com | reservations@outreachtours.com";

            var notesHtml = string.IsNullOrWhiteSpace(notes) ? "" : $@"
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(255,255,255,0.02);border:1px solid {BorderColor};border-radius:12px;"">
                    <tr>
                        <td style=""padding:16px 20px;"">
                            <p style=""margin:0;font-size:12px;color:{PrimaryGold};font-weight:600;"">Guest Preferences / Notes</p>
                            <p style=""margin:6px 0 0 0;font-size:13px;color:{TextWhite};line-height:1.5;"">{EscapeHtml(notes)}</p>
                        </td>
                    </tr>
                </table>";

            var body = $@"
                <div style=""border-bottom:2px solid {PrimaryGold};padding-bottom:12px;margin-bottom:20px;"">
                    <span style=""font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:{PrimaryGold};font-weight:700;"">Official Agency Booking Order</span>
                    <h1 style=""margin:6px 0 0 0;font-size:22px;font-weight:700;color:{TextWhite};"">{EscapeHtml(hotelName)} — Reservation Desk</h1>
                </div>

                <p style=""margin:0 0 20px 0;font-size:14px;color:{TextMuted};line-height:1.5;"">
                    Please find below the guaranteed agency room reservation order for our client with Outreach Tours.
                </p>

                <!-- Order Details Card -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(255,255,255,0.03);border:1px solid {BorderColor};border-radius:12px;overflow:hidden;"">
                    <tr>
                        <td style=""padding:20px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""padding-bottom:12px;border-bottom:1px solid {BorderColor};"">
                                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                            <tr>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Voucher / Order Number</p>
                                                    <p style=""margin:4px 0 0 0;font-size:16px;font-weight:700;color:{PrimaryGold};"">{EscapeHtml(voucherNumber)}</p>
                                                </td>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Booking Agency</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;font-weight:600;color:{TextWhite};"">Outreach Tours &amp; Safaris</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                            <tr>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Lead Guest Name</p>
                                                    <p style=""margin:4px 0 0 0;font-size:15px;font-weight:700;color:{TextWhite};"">{EscapeHtml(clientName)}</p>
                                                </td>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Occupancy</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};font-weight:600;"">{guestsCount} Guest(s) • {roomsCount} Room(s)</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Reserved Room Category</p>
                                        <p style=""margin:4px 0 0 0;font-size:14px;font-weight:600;color:{TextWhite};"">{EscapeHtml(roomType)}</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;border-bottom:1px solid {BorderColor};"">
                                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                            <tr>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Check-In</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};font-weight:600;"">{EscapeHtml(checkIn)}</p>
                                                </td>
                                                <td width=""50%"">
                                                    <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Check-Out</p>
                                                    <p style=""margin:4px 0 0 0;font-size:14px;color:{TextWhite};font-weight:600;"">{EscapeHtml(checkOut)}</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding-top:12px;"">
                                        <p style=""margin:0;font-size:11px;color:{TextMuted};text-transform:uppercase;letter-spacing:1px;"">Agreed Package Valuation</p>
                                        <p style=""margin:4px 0 0 0;font-size:22px;font-weight:700;color:{PrimaryGold};"">KES {formattedAmount}</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <!-- Special Requests -->
                {notesHtml}

                <!-- Billing Instructions -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom:24px;background-color:rgba(16,185,129,0.05);border:1px solid rgba(16,185,129,0.2);border-radius:12px;"">
                    <tr>
                        <td style=""padding:16px 20px;"">
                            <p style=""margin:0;font-size:12px;color:#10b981;font-weight:700;"">💳 Billing &amp; Settlement Instructions</p>
                            <p style=""margin:6px 0 0 0;font-size:12px;color:{TextMuted};line-height:1.5;"">
                                This reservation is guaranteed under the corporate agency account of <strong style=""color:{TextWhite};"">Outreach Tours &amp; Safaris Ltd</strong>. Bill room charges and meal plans to our account. Extras to be settled by guest directly unless specified.
                            </p>
                        </td>
                    </tr>
                </table>

                <p style=""margin:0;font-size:12px;color:{TextMuted};line-height:1.5;"">
                    Please reply to confirm receipt and advise your internal confirmation number. For any queries, contact our operations desk at operations@outreachtours.com.
                </p>";

            var html = WrapInLayout(body, $"Agency Booking Order — {hotelName} ({voucherNumber})");
            return (plain, html);
        }

        private static string EscapeHtml(string input)
        {
            return System.Net.WebUtility.HtmlEncode(input ?? "");
        }
    }
}
