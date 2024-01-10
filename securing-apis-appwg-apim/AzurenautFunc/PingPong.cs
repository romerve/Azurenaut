using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzurenautFunc
{
    public class PingPong
    {
        private readonly ILogger _logger;

        public PingPong(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PingPong>();
        }

        [Function("PingPong")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Azurenaut PONG!");

            return response;
        }
    }
}
