namespace OnlineNotes.Middleware
{
    public class RequestMiddleware
    {
        private readonly ILogger<RequestMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestMiddleware(RequestDelegate next, ILogger<RequestMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var start = DateTime.UtcNow;
            await _next.Invoke(context);
            _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path} at {DateTime.Now}, request time: {(DateTime.UtcNow - start).TotalMilliseconds}ms");
        }
    }
}
