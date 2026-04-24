using MailKit.Net.Imap;
using MailKit.Security;
using MailKit;
using MimeKit;
using HattmakarenWebbAppGrupp03.Models;

namespace HattmakarenWebbAppGrupp03.Services
{
    public class EmailService
    {
        private readonly string _email = "hattmakaren1@outlook.com";
        // Jag har tagit bort eventuella dolda mellanslag i ditt lösenord här under
        private readonly string _password = "dxiijljssxzphgny";

        public async Task<List<Email>> GetLatestEmailsAsync()
        {
            var emails = new List<Email>();

            using (var client = new ImapClient())
            {
                // 1. Anslut till Outlooks server
                await client.ConnectAsync("outlook.office365.com", 993, SecureSocketOptions.SslOnConnect);

                // --- SECURITY FIX START ---
                // Vi tar bort XOAUTH2 för att tvinga MailKit att använda "Basic Authentication" 
                // med ditt 16-siffriga applösenord. Detta löser ofta "AUTHENTICATE failed".
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                // --- SECURITY FIX END ---

                // 2. Logga in
                await client.AuthenticateAsync(_email, _password);

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                // 3. Hämta de 15 senaste mailen
                for (int i = inbox.Count - 1; i >= Math.Max(0, inbox.Count - 15); i--)
                {
                    var msg = await inbox.GetMessageAsync(i);
                    emails.Add(new Email
                    {
                        Subject = msg.Subject ?? "(Inget ämne)",
                        SenderEmail = msg.From.ToString(),
                        Body = msg.TextBody ?? "Endast HTML-innehåll",
                        ReceivedDate = msg.Date.DateTime,
                        MessageId = msg.MessageId
                    });
                }

                await client.DisconnectAsync(true);
            }
            return emails;
        }
    }
}