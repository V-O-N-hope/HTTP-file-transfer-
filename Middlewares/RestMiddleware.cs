using Microsoft.AspNetCore.Mvc;
using WebAplic_test.Classes;

namespace WebAplic_test.Middlewares
{
	public class RestMiddleware
	{
		private readonly RequestDelegate _next;

		public RestMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			//await _next(context);

			var path = context.Request.Path;
			Console.WriteLine("RestMiddleware: " + path);

			var response = context.Response;
			var request = context.Request;
			string? cookieUserName = request.Cookies["username"];
			string? cookieUserPassword = request.Cookies["password"];
			string answer = GeneralInfo.serverAnswer(cookieUserName, cookieUserPassword);
			response.ContentType = "text/html; charset=utf-8";
			if (answer == "ok")
			{
				string userPath = "users/" + cookieUserName;
				if (!Directory.Exists(userPath))
					Directory.CreateDirectory(userPath);
			}
			if (path.ToString().StartsWith($"/users/{cookieUserName}/") && answer == "ok")
			{
				string sPath = path.ToString().Substring(1);

				if (Directory.Exists(sPath))
				{
					if (request.Method == "GET")
					{
						var folder = new DirectoryInfo(sPath);
						var files = folder.GetFiles().Select(file => new
						{
							Name = Path.GetFileNameWithoutExtension(file.Name),
							Extension = file.Extension,
							LastModified = file.LastWriteTime
						});

						var folders = folder.GetDirectories().Select(dir => new
						{
							Name = dir.Name,
							Extension = "folder",
							LastModified = dir.LastWriteTime
						});

						var result = files.Concat(folders).ToList();

						await response.WriteAsJsonAsync(result);
					}
					else if (request.Method == "POST")
					{
						if (request.HasFormContentType && request.Form.Files.Any())
						{
							var file = request.Form.Files.First();
							var filePath = Path.Combine(sPath, file.FileName);
							using var fileStream = new FileStream(filePath, FileMode.Create);
							await file.CopyToAsync(fileStream);
							await response.WriteAsync("Файл успешно загружен");
						}
					}
					else if (Directory.Exists(sPath) && request.Method == "DELETE")
					{
						Console.WriteLine("Зашли в удаление папки");
						Console.WriteLine("Путь: " + sPath);
						try
						{
							Directory.Delete(sPath);
							await response.WriteAsync("Папка успешно удалена");
						}
						catch (Exception ex)
						{
							Console.WriteLine("Ошибка удаления");
							response.StatusCode = 500; // Internal Server Error
							await response.WriteAsync($"Ошибка удаления паппки: {ex.Message}");
						}
					}
				}
				else if (File.Exists(sPath) && request.Method == "DELETE")
				{
					Console.WriteLine("Зашли в удаление файла");
					Console.WriteLine("Путь файла: " + sPath);
					
					try
					{
						File.Delete(sPath);
						await response.WriteAsync("Файл успешно удален");
					}
					catch (Exception ex)
					{
						response.StatusCode = 500; // Internal Server Error
						await response.WriteAsync($"Ошибка удаления файла: {ex.Message}");
					}
				}
				else
				{
					string answerR = "temp";
					await response.WriteAsJsonAsync(answerR);
				}
			}
			else
				await _next(context);
		}
	}
}
