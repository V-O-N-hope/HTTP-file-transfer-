using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using WebAplic_test.Classes;

namespace WebAplic_test.Middlewares
{
	public class RegisterMiddleware
	{
		private readonly RequestDelegate _next;

		public RegisterMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{

			var path = context.Request.Path;
			var fullPath = $"pages/{path}";
			var response = context.Response;
			var request = context.Request;
			response.ContentType = "text/html; charset=utf-8";
			if (path == "/user_reg" && request.Method == "POST")
			{
				string message = shouldAddUser(context);
				await response.WriteAsync(message);
			}
			else if (path == "/user_add" && request.Method == "POST")
			{
				addUser(context);
			}
			else
			{
				await _next.Invoke(context);
			}
		}
		private static string shouldAddUser(HttpContext context)
		{
			var request = context.Request;
			List<User> users = new List<User>();

			if (File.Exists(GeneralInfo.jsonPath))
			{
				string localJson = File.ReadAllText(GeneralInfo.jsonPath);
				if (!string.IsNullOrEmpty(localJson))
				{
					users = JsonSerializer.Deserialize<List<User>>(localJson);
				}
			}

			if (!File.Exists(GeneralInfo.jsonPath))
			{
				using (File.Create(GeneralInfo.jsonPath))
				{
					// Пустой блок. Просто нужно открыть и закрыть поток FileStream,
					// чтобы файл был создан и поток был освобожден.
				}
			}

			string? name = request.Form["name"];
			string? email = request.Form["email"];
			string? password = request.Form["password"];
			string? uniqKey = request.Form["uniqKey"];

			if (users.Any(u => u.name.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
				u.email.Equals(email, StringComparison.InvariantCultureIgnoreCase)))
			{
				return "user_exists";
			}
			return "all_ok";
		}

		private static void addUser(HttpContext context)
		{
			var request = context.Request;
			List<User> users = new List<User>();

			if (File.Exists(GeneralInfo.jsonPath))
			{
				string localJson = File.ReadAllText(GeneralInfo.jsonPath);
				if (!string.IsNullOrEmpty(localJson))
				{
					users = JsonSerializer.Deserialize<List<User>>(localJson);
				}
			}

			if (!File.Exists(GeneralInfo.jsonPath))
			{
				using (File.Create(GeneralInfo.jsonPath))
				{
					// Пустой блок. Просто нужно открыть и закрыть поток FileStream,
					// чтобы файл был создан и поток был освобожден.
				}
			}

			string? name = request.Form["name"];
			string? email = request.Form["email"];
			string? password = request.Form["password"];
			string? uniqKey = request.Form["uniqKey"];
			User newUser = new User()
			{
				name = name,
				email = email,
				password = password,
				uniqKey = uniqKey,
				isConfirmed = false
			};
			users.Add(newUser);

			// Сохраняем список пользователей обратно в файл users.json
			using (StreamWriter sw = File.CreateText(GeneralInfo.jsonPath))
			{
				string json = JsonSerializer.Serialize(users);
				sw.Write(json);
			}
			//Console.WriteLine(uniqKey);
			string link = $"This message is from the MyDrive web-site <br>" +
				$"You've get this mail, because your email adress menthioned <br>" +
				$"To confrig to the page /email.verification and put this token: <br>" +
				uniqKey;
			MailMessage mailMessage = GeneralInfo.CreateMail(email, link);
			GeneralInfo.SendMail(mailMessage);
		}

	}
}
