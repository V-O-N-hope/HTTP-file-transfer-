using System.Text.Json;
using WebAplic_test.Classes;

namespace WebAplic_test.Middlewares
{
	public class UserVerificationMiddleware
	{
		private readonly RequestDelegate _next;
		
		public UserVerificationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var path = context.Request.Path;
			var response = context.Response;
			var request = context.Request;
			response.ContentType = "text/html; charset=utf-8";

			if (path == "/user_verify" && request.Method == "POST")
			{
				string token = request.Form["token"];
				if (isAdded(token))
				{
					await response.WriteAsync("ok");
				}
				else
				{
					await response.WriteAsync("invalid");
				}
			}
			else
			{
				await _next(context);
			}
		}


		private static bool isAdded(string uniqKey)
		{
			bool answer = false;
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
					if (user.uniqKey == uniqKey)
					{
						user.isConfirmed = true;
						answer = true;
						break;
					}
				}

				using (StreamWriter sw = File.CreateText(GeneralInfo.jsonPath))
				{
					string json = JsonSerializer.Serialize(users);
					sw.Write(json);
				}
				return answer;
			}
			else
				return answer;
		}
	}
}
