using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Core;

namespace InventoryFunction
{
    public class AddInventory
    {
        private readonly InventoryService _inventoryService;

        public AddInventory(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [FunctionName("AddInventory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "inventories/{companyName}")] HttpRequest req,
            [CosmosDB(
                databaseName: "IMS",
                containerName: "IMS",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log,
            string companyName)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Inventory newInventory = new()
            {
                id = Guid.NewGuid().ToString(),
                name = req.Query["inventoryName"],
                companyName = companyName
            };

            var result = await _inventoryService.AddInventoryAsync(newInventory, client, log);

            string responseMessage = $"Added inventory \"{newInventory.name}\" to company \"{companyName}\" ";

            if (result) {
                return new OkObjectResult(responseMessage);
            }
            return new BadRequestObjectResult("Failed to add new inventory.");  
        }
    }
}
