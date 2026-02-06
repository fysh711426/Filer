using Microsoft.AspNetCore.Mvc;

namespace Filer.Results
{
    public class PushStreamResult : IActionResult
    {
        private readonly string _contentType;
        private readonly Func<Stream, CancellationToken, Task> _onStream;
        
        public PushStreamResult(Func<Stream, CancellationToken, Task> onStreamAvailable, string contentType)
        {
            _contentType = contentType;
            _onStream = onStreamAvailable;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentType = _contentType;
            await _onStream(context.HttpContext.Response.Body, context.HttpContext.RequestAborted);
        }
    }
}