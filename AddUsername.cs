using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace InventoryFunction
{
    public class AddUsername
    {
        private readonly InventoryService _inventoryService;

        public AddUsername(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [FunctionName("AddUsername")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "inventories/{companyName}/{inventoryName}/usernames")] HttpRequest req,
            [CosmosDB(
                databaseName: "IMS",
                containerName: "IMS",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log,
            string companyName,
            string inventoryName)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic? data = JsonConvert.DeserializeObject(requestBody);

            if (data == null || data!.name == null)
            {
                return new BadRequestObjectResult("Please provide 'username' in the request body.");
            }

            User newUser = new()
            {
                id = data!.name,
                name = data!.name,
                companyName = companyName,
                inventoryName = inventoryName
            };

            var result = await _inventoryService.AddUserAsync(newUser, client, log);

            string responseMessage = $"Added username \"{newUser.name}\" to \"{companyName}\"/\"{inventoryName}\"";
            log.LogInformation(responseMessage);

            if (result) {
                return new OkObjectResult(responseMessage);
            }
            return new BadRequestObjectResult("Failed to add new username.");  
        }
    }
}
