using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace EventPlanning.Models
{
    public class EmailService
    {
        static SettingContext db = new SettingContext();
        public static async Task SendAsync(IdentityMessage message)
        {
            var emailMessage = new MimeKit.MimeMessage();

            emailMessage.From.Add(new MimeKit.MailboxAddress("Администрация сайта", "nikita.zabeyda@gmail.com"));
            emailMessage.To.Add(new MimeKit.MailboxAddress("admin", message.Destination));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 25, false);
                await client.AuthenticateAsync(db.Settings.First().SendEmail, db.Settings.First().SendPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public static bool IsConnection()
        {
            try
            {
                bool result;
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ConnectAsync("smtp.gmail.com", 25, false);
                    client.AuthenticateAsync(db.Settings.First().SendEmail, db.Settings.First().SendPassword);
                    result = client.IsConnected && client.IsAuthenticated;
                    client.DisconnectAsync(true);
                }
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}