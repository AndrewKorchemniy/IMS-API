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

namespace InventoryFunction
{
    public class DeleteItem
    {
        private readonly InventoryService _inventoryService;

        public DeleteItem(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [FunctionName("DeleteItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "inventories/{companyName}/{inventoryName}/items")] HttpRequest req,
            [CosmosDB(
                databaseName: "IMS",
                containerName: "IMS",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log,
            string companyName,
            string inventoryName)
        {
            string itemId = req.Query["id"];

            var result = await _inventoryService.DeleteItemAsync(itemId, companyName, inventoryName, client, log);

            return result;
        }
    }
}
