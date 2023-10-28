using ChatApp.Client.Data;

namespace ChatApp.Client.Extensions
{
    public class HeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public HeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var handlerData = this.httpContextAccessor.HttpContext.RequestServices.GetRequiredService<HandlerData>();

            if (handlerData.HeaderValue != null && handlerData.HeaderName != null)
            {
                request.Headers.Add(handlerData.HeaderName, handlerData.HeaderValue);
            }
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}
