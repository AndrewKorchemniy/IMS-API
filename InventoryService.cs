using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryFunction
{
    public class InventoryService
    {

        private ILogger _logger;

        public InventoryService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> AddInventoryAsync(Inventory inventory, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("IMS").GetContainer("Inventories");
            var result = await container.CreateItemAsync(inventory);

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                log.LogInformation($"Added inventory {inventory.name} in {inventory.companyName}");
                return true;
            }

            log.LogInformation($"Failed to add inventory {inventory.name} in {inventory.companyName}");
            return false;
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesAsync(string companyName, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("IMS").GetContainer("Inventories");
            QueryDefinition queryDefinition = new QueryDefinition(
                $"SELECT * FROM Inventories i WHERE i.companyName = @companyName")
                .WithParameter("@companyName", companyName);

            List<Inventory> inventories = new();
            using (FeedIterator<Inventory> resultSet = container.GetItemQueryIterator<Inventory>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    inventories.AddRange(await resultSet.ReadNextAsync());
                }
            }

            return inventories;
        }

        public async Task<bool> AddUserAsync(User user, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("IMS").GetContainer("Usernames");
            var result = await container.CreateItemAsync(user);

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                log.LogInformation($"Added username {user.name} in {user.companyName}/{user.inventoryName}");
                return true;
            }

            log.LogInformation($"Failed to add username {user.name} in {user.companyName}/{user.inventoryName}");
            return false;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(string companyName, string inventoryName, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("IMS").GetContainer("Usernames");
            log.LogInformation($"Searching for username in {companyName}/{inventoryName}");
            QueryDefinition queryDefinition = new QueryDefinition(
                $"SELECT * FROM Usernames i WHERE i.companyName = @companyName AND i.inventoryName = @inventoryName")
                .WithParameter("@companyName", companyName)
                .WithParameter("@inventoryName", inventoryName);

            List<User> users = new();
            using (FeedIterator<User> resultSet = container.GetItemQueryIterator<User>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    users.AddRange(await resultSet.ReadNextAsync());
                }
            }

            return users;
        }

        public async Task<bool> AddItemAsync(Item item, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("IMS").GetContainer("Items");
            log.LogInformation($"Add item {item.name} in {item.companyName}/{item.inventoryName}");
            var result = await container.CreateItemAsync(item);

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                log.LogInformation($"Added item {item.name} in {item.companyName}/{item.inventoryName}");
                return true;
            }

            log.LogInformation($"Failed to add item {item.name} in {item.companyName}/{item.inventoryName}");
            return false;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(string companyName, string inventoryName, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("IMS").GetContainer("Items");
            log.LogInformation($"Searching for items in {companyName}/{inventoryName}");
            QueryDefinition queryDefinition = new QueryDefinition(
                $"SELECT * FROM Items i WHERE i.companyName = @companyName AND i.inventoryName = @inventoryName")
                .WithParameter("@companyName", companyName)
                .WithParameter("@inventoryName", inventoryName);

            List<Item> items = new();
            using (FeedIterator<Item> resultSet = container.GetItemQueryIterator<Item>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    items.AddRange(await resultSet.ReadNextAsync());
                }
            }

            return items;
        }

        public async Task<ActionResult> DeleteItemAsync(string itemId, string companyName, string inventoryName, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("IMS").GetContainer("Items");
            var result = await container.DeleteItemAsync<Item>(itemId, new PartitionKey(companyName));

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                log.LogInformation($"Successfully deleted item {itemId} in {companyName}/{inventoryName}");
                return new OkObjectResult($"Successfully deleted item {itemId} in {companyName}/{inventoryName}");
            }

            log.LogInformation($"Failed to delete item {itemId} in {companyName}/{inventoryName}");
            return new BadRequestResult();
        }
    }
}