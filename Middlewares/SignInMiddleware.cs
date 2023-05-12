using System.Net;
using System.Net.Mail;
using System.Text.Json;
using WebAplic_test.Classes;

namespace WebAplic_test.Middlewares
{
	public class SignInMiddleware
	{
		private readonly RequestDelegate _next;

		public SignInMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var path = context.Request.Path;
			var response = context.Response;
			var request = context.Request;
			response.ContentType = "text/html; charset=utf-8";
			if (path == "/login" && request.Method == "POST")
			{
				var name = request.Form["name"];
				var password = request.Form["password"];
				//var answer = ;
				await response.WriteAsync(serverAnswer(name, password));
			}
			else
			{
				await _next(context);
			}
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
						if (user.isConfirmed)
						{
							user.isConfirmed = false;
							using (StreamWriter sw = File.CreateText(GeneralInfo.jsonPath))
							{
								string json = JsonSerializer.Serialize(users);
								sw.Write(json);
							}
							string link = $"This message is from the MyDrive web-site because some-one entered to this web page <br>" +
				$"You need to put this token for entering again <br>" +
				$"To confrig to the page /email.verification and put this token: <br>" +
				user.uniqKey;
							MailMessage mailMessage = GeneralInfo.CreateMail(user.email, link);
							GeneralInfo.SendMail(mailMessage);
							return "ok";
						}
						else
							return "not_confirmed";
					}
				}
			}
			return "not_found";
		}
		
	}
}
