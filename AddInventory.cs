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
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data == null || data.inventoryName == null)
            {
                return new BadRequestObjectResult("Please provide 'inventoryName' in the request body.");
            }

            Inventory newInventory = new()
            {
                id = data!.inventoryName,
                name = data!.inventoryName,
                companyName = companyName
            };

            var result = await _inventoryService.AddInventoryAsync(newInventory, client, log);

            string responseMessage = $"Added inventory \"{newInventory.name}\" to company \"{companyName}\" ";
            log.LogInformation(responseMessage);

            if (result) {
                return new OkObjectResult(responseMessage);
            }
            return new BadRequestObjectResult("Failed to add new inventory.");  
        }
    }
}
