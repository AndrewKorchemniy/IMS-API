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
    public class GetItems
    {
        private readonly InventoryService _inventoryService;

        public GetItems(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [FunctionName("GetItems")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "inventories/{companyName}/{inventoryName}/items")] HttpRequest req,
            [CosmosDB(
                databaseName: "IMS",
                containerName: "IMS",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log,
            string companyName,
            string inventoryName)
        {
            var items = await _inventoryService.GetItemsAsync(companyName, inventoryName, client, log);

            string username = req.Query["username"];

            log.LogInformation($"{username} queried for items from {companyName}/{inventoryName}");

            return new OkObjectResult(items);
        }
    }
}
