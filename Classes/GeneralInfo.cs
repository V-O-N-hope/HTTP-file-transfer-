using System.Net.Mail;
using System.Net;
using System.Text.Json;

namespace WebAplic_test.Classes
{
	public static  class GeneralInfo
	{
		public const string jsonPath = "users.json";
		public const string emailFrom = "ksisdrive@gmail.com";
		public const string pass = "wubddgtpnvcznvwd";

		public static MailMessage CreateMail(string emailTo, string body)
		{
			var from = new MailAddress(GeneralInfo.emailFrom, "ksis-drive");
			var to = new MailAddress(emailTo);
			var mail = new MailMessage(from, to);
			mail.Subject = "Verifying email";
			mail.Body = body;
			mail.IsBodyHtml = true;
			return mail;
		}

		public static void SendMail(MailMessage message)
		{
			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
			smtpClient.Credentials = new NetworkCredential(emailFrom, pass);
			smtpClient.EnableSsl = true;

			smtpClient.Send(message);
		}

		public static string serverAnswer(string name, string password)
		{
			List<User> users = new List<User>();

			if (File.Exists(GeneralInfo.jsonPath))
			{
				string localJson = File.ReadAllText(GeneralInfo.jsonPath);
				if (!string.IsNullOrEmpty(localJson))
				{
					users = JsonSerializer.Deserialize<List<User>>(localJson);
				}
				foreach (var user in users)
				{
					if (user.name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
						user.password == password)
					{
						return "ok";
					}
				}

				return "check_fields";
			}
			else
			{
				return "not_exists";
			}
		}
	}
}
