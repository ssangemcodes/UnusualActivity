using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnusualActivity.Common;

namespace OptionRadarUI_Ag.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OptionsActivityController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string EndpointUri = "https://optionvolumes.documents.azure.com:443/";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "cYWKVAv33322EDySCxiMzVugsdge3Wc9Vh6eSOKTHK3p1WEyD8rRJLAmaRhzVbnmm9u9bnFRIf9DFIzo2CguRQ==";

        // The Cosmos client instance
        private static CosmosClient cosmosClient;

        // The database we will create
        private static Database database;

        // The container we will create.
        private static Container container;

        // The name of the database and container we will create
        private static string databaseId = "db";
        private static string containerId = "cOptionVolume";

        private readonly ILogger<OptionsActivityController> _logger;

        public OptionsActivityController(ILogger<OptionsActivityController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetUnusualActivity")]
        public string GetUnusualActivity()
        {
            var uoActivityDocuments = new List<UOActivityDocument>();
            cosmosClient = new CosmosClient("https://optionvolumes.documents.azure.com:443/", "cYWKVAv33322EDySCxiMzVugsdge3Wc9Vh6eSOKTHK3p1WEyD8rRJLAmaRhzVbnmm9u9bnFRIf9DFIzo2CguRQ==", new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            container = database.CreateContainerIfNotExistsAsync(containerId, "/USymbol", 400).Result;

            var sqlQueryText = "SELECT * FROM c";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);
            _logger.LogTrace("Running query: {0}\n", sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<UOActivityDocument> queryResultSetIterator = container.GetItemQueryIterator<UOActivityDocument>(queryDefinition);

            List<UOActivityDocument> activityDocs = new List<UOActivityDocument>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<UOActivityDocument> currentResultSetIterator = queryResultSetIterator.ReadNextAsync().Result;

                foreach (UOActivityDocument activityDoc in currentResultSetIterator)
                {
                    activityDocs.Add(activityDoc);
                    Console.WriteLine("\tRead {0}\n", activityDoc);
                    _logger.LogTrace("\tRead {0}\n", activityDoc);
                }
            }

            return JsonConvert.SerializeObject(activityDocs);
        }

        [HttpGet]
        public IEnumerable<MakeModel> Get()
        {
            var makeModelList = new List<MakeModel>();
            makeModelList.Add(new MakeModel { Make="Honda", Model ="Bonda", Price = 1.887 });
            return makeModelList;
        }
    }

    public class MakeModel
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public double Price { get; set; }

    }
}
