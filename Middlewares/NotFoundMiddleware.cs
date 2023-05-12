namespace HTTP_FILE_transfer_1.Middlewares
{
    public class NotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
			await _next(context);

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = 404;
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync("error_pages/404.html");
            }
            
        }

    }
}
