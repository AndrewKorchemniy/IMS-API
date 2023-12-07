using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
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
            log.LogInformation($"Add inventory {inventory.name} in {inventory.companyName}");
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
            log.LogInformation($"Searching for Pending Jokes");
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
            log.LogInformation($"Add user {user.name} in {user.companyName}/{user.inventoryName}");
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
            log.LogInformation($"Searching for Usernames");
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
            log.LogInformation($"Searching for Items");
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
            log.LogInformation($"Delete item {itemId} in {companyName}/{inventoryName}");
            var result = await container.DeleteItemAsync<Item>(itemId, new PartitionKey(companyName));

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                log.LogInformation($"Successfully deleted item {itemId} in {companyName}/{inventoryName}");
                return new OkObjectResult($"Successfully deleted item {itemId} in {companyName}/{inventoryName}");
            }

            log.LogInformation($"Failed to delete item {itemId} in {companyName}/{inventoryName}");
            return new BadRequestResult();
        }

        // Exmaples -->




        //public async Task<Joke> GetRandomJoke(CosmosClient client, ILogger log)
        //{
        //    Container container = client.GetDatabase("Jokes").GetContainer("items");

        //    log.LogInformation($"Searching for Joke Count");

        //    QueryDefinition queryDefinition = new QueryDefinition(
        //        "SELECT value Count(i) FROM items i");

        //    int count = 0;
        //    using (FeedIterator<int> resultSet = container.GetItemQueryIterator<int>(queryDefinition))
        //    {
        //        count = (await resultSet.ReadNextAsync()).First();
        //    }
        //    log.LogInformation($"{count} jokes found");

        //    // Random number between 0 and count
        //    var rnd = new Random();
        //    int offset = rnd.Next(count);

        //    log.LogInformation($"Grabbing joke {offset} of {count}");


        //    QueryDefinition queryDefinitionJoke = new QueryDefinition(
        //        $"SELECT * FROM items i OFFSET {offset} LIMIT 1");

        //    Joke? joke = null;
        //    using (FeedIterator<Joke> resultSet = container.GetItemQueryIterator<Joke>(queryDefinitionJoke))
        //    {
        //        joke = (await resultSet.ReadNextAsync()).First();
        //    }

        //    return joke;

        //}

        //public async Task<IEnumerable<Joke>> GetPendingJokes(CosmosClient client, ILogger log)
        //{
        //    Container container = client.GetDatabase("Jokes").GetContainer("PendingItems");

        //    log.LogInformation($"Searching for Pending Jokes");

        //    QueryDefinition queryDefinition = new QueryDefinition(
        //        "SELECT * FROM PendingItems i");

        //    List<Joke> jokes = new();
        //    using (FeedIterator<Joke> resultSet = container.GetItemQueryIterator<Joke>(queryDefinition))
        //    {
        //        while (resultSet.HasMoreResults)
        //        {
        //            jokes.AddRange(await resultSet.ReadNextAsync());
        //        }
        //    }

        //    return jokes;

        //}

        //public async Task<bool> DeletePendingJoke(Joke joke, CosmosClient client, ILogger log)
        //{
        //    Container container = client.GetDatabase("Jokes").GetContainer("PendingItems");

        //    log.LogInformation($"Delete Pending Joke {joke.id} in {joke.author}");

        //    var result = await container.DeleteItemAsync<Joke>(joke.id, new PartitionKey(joke.author));

        //    if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
        //    {
        //        log.LogInformation($"Deleted Pending Joke {joke.id} in {joke.author}");

        //        return true;
        //    }

        //    log.LogInformation($"Failed to Delete Pending Joke {joke.id} in {joke.author}");
        //    return false;
        //}

        //public async Task<bool> AddJoke(Joke joke, CosmosClient client, ILogger log)
        //{
        //    Container container = client.GetDatabase("Jokes").GetContainer("items");

        //    log.LogInformation($"Add Joke {joke.id} in {joke.author}");

        //    var result = await container.CreateItemAsync<Joke>(joke);

        //    if (result.StatusCode == System.Net.HttpStatusCode.Created)
        //    {
        //        log.LogInformation($"Added Joke {joke.id} in {joke.author}");

        //        return true;
        //    }

        //    log.LogInformation($"Failed to Add Joke {joke.id} in {joke.author}");
        //    return false;
        //}

        //public async Task<Joke?> GetJoke(string id, CosmosClient client, ILogger log)
        //{
        //    Container container = client.GetDatabase("Jokes").GetContainer("items");

        //    QueryDefinition queryDefinitionJoke = new QueryDefinition(
        //        @$"SELECT * FROM items i WHERE i.id=""{id}""");

        //    Joke? joke = null;
        //    using (FeedIterator<Joke> resultSet = container.GetItemQueryIterator<Joke>(queryDefinitionJoke))
        //    {
        //        joke = (await resultSet.ReadNextAsync()).FirstOrDefault();
        //    }

        //    return joke;
        //}

    }
}