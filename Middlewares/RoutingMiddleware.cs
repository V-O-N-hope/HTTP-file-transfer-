using System.Text.Json;
using WebAplic_test.Classes;

namespace WebAplic_test.Middlewares
{
	public class RoutingMiddleware
	{
		private readonly RequestDelegate _next;
		public RoutingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var path = context.Request.Path;
			Console.WriteLine("Routing: " + path);

			var response = context.Response;
			var request = context.Request;
			string? cookieUserName = request.Cookies["username"];
			string? cookieUserPassword = request.Cookies["password"];
			string answer = GeneralInfo.serverAnswer(cookieUserName, cookieUserPassword);
			response.ContentType = "text/html; charset=utf-8";
			if (path == "/registeredUserPage.html" && cookieUserName != null && cookieUserPassword != null && answer == "ok")
			{
				string userPath = "users/" + cookieUserName;
				if (!Directory.Exists(userPath))
					Directory.CreateDirectory(userPath);

				await response.SendFileAsync("pages" + path);
			}
			else if (path == "/" && cookieUserName != null && cookieUserPassword != null && answer == "ok")
			{
				await response.SendFileAsync("pages/registeredUserPage.html");
			}
			else if (cookieUserName != null && cookieUserPassword != null && answer != "ok")
			{
				//delete cookies 
				response.Cookies.Delete("username");
				response.Cookies.Delete("password");
				response.Redirect("/index.html");
			}
			else if (path == "/" && cookieUserName == null && cookieUserPassword == null)
			{
				await response.SendFileAsync("pages/index.html");
			}
			else if (cookieUserName == null && cookieUserPassword == null)
			{
				if (File.Exists("pages/" + path))
					await response.SendFileAsync("pages" + path);
				else
					response.StatusCode = 404;
			}
			else
				await _next(context); 

		}

		
	}
}
