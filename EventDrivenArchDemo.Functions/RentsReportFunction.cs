using System;
using System.Text.Json;
using EventDrivenArchDemo.Functions.Model;
using EventDrivenArchDemo.Functions.Model.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EventDrivenArchDemo.Functions
{
    public class RentsReportFunction
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public RentsReportFunction(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<RentsReportFunction>();
            _httpClient = httpClientFactory.CreateClient();
        }

        [Function("RentsReport")]
        [CosmosDBOutput(
            databaseName: "BookRentalShop",
            containerName: "RentsReport",
            CreateIfNotExists = true,
            PartitionKey = "/ClientId",
            Connection = "CosmosDb")]
        public async Task<RentsReport?> Run([RabbitMQTrigger("RentCreated_LibraryReportsFunction", ConnectionStringSetting = "RabbitMQ")] RentCreatedEvent rentCreatedEvent)
        {            
            var apiUrl = $"http://eventdrivenarchdemo.api/api/rents/{rentCreatedEvent.Id}";
            RentsReport? report = null;

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                report = JsonSerializer.Deserialize<RentsReport>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch rent details from API.");
                return null;
            }

            if (report != null)
            {
                report.id = report.RentId.ToString(); // Cosmos DB requires 'id' property
                report.CreatedAt = DateTime.UtcNow;                
                _logger.LogInformation($"Rent report stored in CosmosDB for RentId: {report.RentId}");
                return report;
            }
            else
            {
                _logger.LogWarning($"No rent details found for RentId: {rentCreatedEvent.Id}");
                return null;
            }
        }
    }
}
