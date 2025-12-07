using API.DTOs.OrderDTO;
using API.Helpers;
using API.Interfaces.Services;
using API.Payloads.Order;
using CloudinaryDotNet.Actions;
using System.Net.Mail;
using System.Net;
using API.Payloads.Auth;

namespace API.Services.Secondary
{
    public class MailService : IMailService
    {
        private IConfiguration _config;
        public MailService(IConfiguration config)
        {
            _config = config;
        }
        private async Task SendMailAsync(string html, string email, string subject)
        {

            GmailSettings gmailSettings = _config.GetSection(nameof(gmailSettings)).Get<GmailSettings>()!;

            var message = new MailMessage();
            message.From = new MailAddress(gmailSettings.Email);
            message.Subject = subject;
            message.To.Add(new MailAddress(email));
            message.Body = html;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient("smtp.gmail.com");

            smtpClient.Port = gmailSettings.Port;
            smtpClient.Credentials = new NetworkCredential(gmailSettings.Email, gmailSettings.Password);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);

        }
        public async Task SendSummaryAsync(OrderMail payload)
        {
            
            string html = SetHTML(payload);

            await SendMailAsync(html, payload.Email, "Your order in Alexpress");
        }
        private static string SetHTML(OrderMail payload)
        {
            string fontStyle = "font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #000000; font-size: 14px;";
            string reset = "margin: 0; padding: 0;";

            string orderContainer = $@"
            <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #ffffff;"">
                <tr>
                    <td align=""center"" style=""padding: 40px 10px;"">
                        <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""max-width: 520px; width: 100%; {fontStyle}"">
                            <!-- Título -->
                            <tr>
                                <td align=""center"" style=""padding-bottom: 20px;"">
                                    <p style=""{reset} font-size: 18px; font-weight: 500; text-align: center;"">{payload.Username}, your order in Alexpress</p>
                                </td>
                            </tr>
            ";

            foreach (var p in payload.OrderedProducts)
            {
                
                orderContainer += $@"
                <tr>
                    <td style=""padding-bottom: 15px;"">
                        <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #f0ffff; border-radius: 8px; overflow: hidden;"">
                            <tr>
                                <!-- Columna Imagen (Ancho fijo para estabilidad) -->
                                <td width=""150"" valign=""top"" style=""width: 150px;"">
                                    <img src=""{p.Image}"" width=""150"" height=""150"" style=""display: block; width: 100%; max-width: 150px; height: 150px; object-fit: cover; border-top-left-radius: 8px; border-bottom-left-radius: 8px;"" alt=""Product"">
                                </td>
                                <!-- Columna Contenido -->
                                <td valign=""top"" style=""padding: 15px;"">
                                    <p style=""{reset} font-weight: bold; font-size: 15px; margin-bottom: 8px; line-height: 1.2;"">{p.Title}</p>
                                    <p style=""{reset} margin-bottom: 4px;"">Price: <span style=""font-weight: normal;"">{p.Price}</span></p>
                                    <p style=""{reset} margin-bottom: 4px;"">Quantity: <span style=""font-weight: normal;"">{p.Quantity}</span></p>
                                    <p style=""{reset} margin-bottom: 4px;"">Shipping: <span style=""font-weight: normal;"">{p.ShippingPrice}</span></p>
                                    <p style=""{reset} margin-top: 8px;"">
                                        Coupon: <span style=""{(p.CouponName != null ? "background-color: #00ffff; padding: 2px 6px; border-radius: 4px;" : "")}"">{(p.CouponName ?? "None")}</span>
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                ";
            }

                        
            orderContainer += $@"
                            <tr>
                                <td style=""padding-top: 10px;"">
                                    <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""15"" style=""background-color: #e0e0e0; border-radius: 4px;"">
                                        <tr>
                                            <td align=""left"">
                                                <p style=""{reset} font-size: 18px; font-weight: bold;"">Total:</p>
                                            </td>
                                            <td align=""right"">
                                                <p style=""{reset} font-size: 18px; font-weight: bold;"">{payload.Summary}$</p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            ";

                string docType = $@"<!DOCTYPE html>
            <html lang=""en"">
            <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Order Confirmation</title>
            </head>
            <body style=""margin: 0; padding: 0; background-color: #ffffff;"">
            {orderContainer}
            </body>
            </html>";

            return docType;

        }
    
        public async Task SendEmailTokenAsync(EmailToken payload, string angularPageName, string actionName, string subject)
        {
            string angularUrl = _config["AngularUrl"]!;

            string encodedEmail = WebUtility.UrlEncode(payload.Email);
            string encodedToken = WebUtility.UrlEncode(payload.Token);

            string url = $"{angularUrl}/{angularPageName}/{encodedToken}?email={encodedEmail}";

            string html = $@"

                <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; text-align: center; padding: 20px;"">
                    <h1 style=""color: #333333; margin-bottom: 10px; font-size: 24px;"">
                        Click the link below to {actionName}
                    </h1>
                    <h2 style=""color: #0078d4; font-weight: normal; font-size: 18px; margin-top: 0; word-break: break-all;"">
                        {url}
                    </h2>
                </div>
            ";

            await SendMailAsync(html, payload.Email, subject);
        }

    }
}
