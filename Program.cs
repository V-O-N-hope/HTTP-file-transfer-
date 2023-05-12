using HTTP_FILE_transfer_1.Middlewares;
using WebAplic_test.Middlewares;

namespace WebAplic_test
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var app = builder.Build();
			app.UseStaticFiles();
			app.UseMiddleware<NotFoundMiddleware>();
			app.UseMiddleware<RegisterMiddleware>();
			app.UseMiddleware<SignInMiddleware>();
			app.UseMiddleware<UserVerificationMiddleware>();
			app.UseMiddleware<RoutingMiddleware>();
			app.UseMiddleware<RestMiddleware>();


			app.Run();
		}

	}
}