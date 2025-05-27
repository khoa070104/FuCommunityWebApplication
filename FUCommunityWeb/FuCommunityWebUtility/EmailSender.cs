using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FuCommunityWebUtility
{
	public class EmailSender : IEmailSender
	{
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var fromEmail = "qqnutnut7@gmail.com";
			var appPassword = "uvhq nwfk tjun mhuw"; 

			var client = new SmtpClient("smtp.gmail.com", 587)
			{
				Credentials = new NetworkCredential(fromEmail, appPassword),
				EnableSsl = true
			};

			var mailMessage = new MailMessage(fromEmail, email, subject, htmlMessage)
			{
				IsBodyHtml = true 
			};
			await client.SendMailAsync(mailMessage);
		}
	}
}
