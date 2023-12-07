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
    public class GetUsername
    {
        private readonly InventoryService _inventoryService;

        public GetUsername(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [FunctionName("GetUsernames")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "inventories/{companyName}/{inventoryName}")] HttpRequest req,
            [CosmosDB(
                databaseName: "IMS",
                containerName: "IMS",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log,
            string companyName,
            string inventoryName)
        {
            var inventories = await _inventoryService.GetUsersAsync(companyName, inventoryName, client, log);

            return new OkObjectResult(inventories);
        }
    }
}
