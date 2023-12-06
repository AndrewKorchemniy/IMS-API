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
    public class GetInventory
    {
        private readonly InventoryService _inventoryService;

        public GetInventory(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [FunctionName("GetInventories")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "inventories/{companyName}")] HttpRequest req,
            [CosmosDB(
                databaseName: "IMS",
                containerName: "IMS",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log,
            string companyName)
        {
            log.LogInformation("C# GetInventories function processed a request.");

            var inventories = await _inventoryService.GetInventoriesAsync(companyName, client, log);

            return new OkObjectResult(inventories);
        }
    }
}
