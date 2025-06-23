using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EventDrivenArchDemo.Functions
{
    public class RentsReportFunction
    {
        private readonly ILogger _logger;

        public RentsReportFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RentsReportFunction>();
        }

        [Function("RentsReport")]
        public void Run([RabbitMQTrigger("RentCreated_LibraryReportsFunction", ConnectionStringSetting = "RabbitMQ")] string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
