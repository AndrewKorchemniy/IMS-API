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
    public class AddItem
    {
        private readonly InventoryService _inventoryService;

        public AddItem(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [FunctionName("AddItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "inventories/{companyName}/{inventoryName}/items")] HttpRequest req,
            [CosmosDB(
                databaseName: "IMS",
                containerName: "IMS",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log,
            string companyName,
            string inventoryName)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Item newItem = JsonConvert.DeserializeObject<Item>(requestBody)!;

            if (newItem == null)
            {
                return new BadRequestObjectResult("Please provide 'item' in the request body.");
            }

            var result = await _inventoryService.AddItemAsync(newItem, client, log);

            string responseMessage = $"Added item \"{newItem.name}\" to \"{companyName}\"/\"{inventoryName}\"";

            if (result) {
                return new OkObjectResult(responseMessage);
            }
            return new BadRequestObjectResult("Failed to add new item.");  
        }
    }
}
