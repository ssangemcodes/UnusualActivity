using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using System.Configuration;
using System.Net;
using System.Linq;
using UnusualActivity.Common;
using System.Threading;

namespace OptionActivityFunction
{
    public static class OptionActivity
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

        // The Cosmos client instance
        private static CosmosClient cosmosClient;

        // The database we will create
        private static Database database;

        // The container we will create.
        private static Container container;

        // The name of the database and container we will create
        private static string databaseId = "db";
        private static string containerId = "cOptionVolume";

        [FunctionName("GetOABySymbol")]
        public static async Task<IActionResult> GetOABySymbol([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {

                string name = req.Query["symbol"];
                var qSymbols = name ?? string.Empty;

                var listOfSymbols = name.Split(',');
                var docs = new List<UOActivityDocument>();
                var client = new HttpClient();

                foreach (var symbol in listOfSymbols)
                {
                    var response = client.GetAsync(string.Format(@"https://api.tdameritrade.com/v1/marketdata/chains?apikey=6ANM3TCMXETNKQLNHSCYJCLYNGKHUHLS%40AMER.OAUTHAP&symbol={0}&contractType=ALL&strikeCount=100",symbol));
                    var responseString = response.Result.Content.ReadAsStringAsync().Result;

                    //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    var data = JsonConvert.DeserializeObject<Security>(responseString);

                    foreach (var expiration in data.Puts)
                    {
                        foreach (var strike in expiration.Value)
                        {
                            foreach (var contract in strike.Value)
                            {
                                var doc = new UOActivityDocument
                                {
                                    USymbol = data.USymbol,
                                    OSymbol = contract.OSymbol,
                                    Id = contract.OSymbol,
                                    Description = contract.Description,
                                    Strike = strike.Key,
                                    StringExpirationDate = expiration.Key,
                                    ExpirationDate = contract.ExpirationDate,
                                    TotalVolume = contract.TotalVolume,
                                    OpenInterest = contract.OpenInterest,
                                    Delta = contract.Delta,
                                    Gamma = contract.Gamma,
                                    Rho = contract.Rho,
                                    Theta = contract.Theta,
                                    Vega = contract.Vega,
                                    ContractType = "Put"
                                };

                                docs.Add(doc);
                            }
                        }
                    }
                    foreach (var expiration in data.Calls)
                    {
                        foreach (var strike in expiration.Value)
                        {
                            foreach (var contract in strike.Value)
                            {
                                var doc = new UOActivityDocument
                                {
                                    USymbol = data.USymbol,
                                    OSymbol = contract.OSymbol,
                                    Id = contract.OSymbol,
                                    Description = contract.Description,
                                    Strike = strike.Key,
                                    StringExpirationDate = expiration.Key,
                                    ExpirationDate = contract.ExpirationDate,
                                    TotalVolume = contract.TotalVolume,
                                    OpenInterest = contract.OpenInterest,
                                    Delta = contract.Delta,
                                    Gamma = contract.Gamma,
                                    Rho = contract.Rho,
                                    Theta = contract.Theta,
                                    Vega = contract.Vega,
                                    ContractType = "Call"
                                };

                                docs.Add(doc);
                            }
                        }
                    }

                    cosmosClient = new CosmosClient("https://optionvolumes.documents.azure.com:443/", "cYWKVAv33322EDySCxiMzVugsdge3Wc9Vh6eSOKTHK3p1WEyD8rRJLAmaRhzVbnmm9u9bnFRIf9DFIzo2CguRQ==", new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
                    database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
                    container = await database.CreateContainerIfNotExistsAsync(containerId, "/USymbol", 400);

                    foreach (var item in docs)
                    {

                        try
                        {
                            // Read the item to see if it exists.  
                            ItemResponse<UOActivityDocument> itemDocResponse = await container.ReadItemAsync<UOActivityDocument>(item.OSymbol, new PartitionKey(item.USymbol));
                            Console.WriteLine("Item in database with id: {0} already exists\n", item.USymbol);
                            log.LogTrace("Item in database with id: {0} already exists\n", item.USymbol);
                        }
                        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                        {
                            // Create an item in the container representing the option activity. Note we provide the value of the partition key for this item, which is underlying symbol
                            ItemResponse<UOActivityDocument> itemDocResponse = await container.CreateItemAsync(item, new PartitionKey(item.USymbol));

                            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemDocResponse.Resource.Id, itemDocResponse.RequestCharge);
                            log.LogTrace("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemDocResponse.Resource.Id, itemDocResponse.RequestCharge);
                        }

                    }

                }
                return new OkObjectResult(JsonConvert.SerializeObject(docs));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [FunctionName("GetOAForAllFromConfig")]
        public static async Task<IActionResult> GetOAForAllFromConfig([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            var docs = new List<UOActivityDocument>();
            try
            {
                var spxSymbols = "ABT|ABBV|ABMD|ACN|ATVI|ADBE|AMD|AAP|AES|AFL|A|APD|AKAM|ALK|ALB|ARE|ALXN|ALGN|ALLE|ALL|AMZN|AMCR|AEE|AAL|AEP|AXP|AIG|AMT|AWK|AMP|ABC|AME|AMGN|APH|ADI|ANSS|ANTM|AON|AOS|APA|AIV|AAPL|AMAT|APTV|ADM|ANET|AJG|AIZ|ATO|ADSK|ADP|AZO|AVB|AVY|AVGO|BKR|BLL|BAC|BK|BAX|BDX|BRK.B|BBY|BIO|BIIB|BLK|BA|BKNG|BWA|BXP|BSX|BMY|BR|BF.B|BEN|CHRW|COG|CDNS|CPB|COF|CAH|CCL|CARR|CAT|CBOE|CBRE|CDW|CE|CNC|CNP|CTL|CERN|CF|CHTR|CVX|CMG|CB|CHD|CI|CINF|CTAS|CSCO|C|CFG|CTXS|CLX|CME|CMS|CTSH|CL|CMCSA|CMA|CAG|CXO|COP|COO|CPRT|CTVA|COST|COTY|CCI|CSX|CMI|CVS|CRM|DHI|DHR|DRI|DVA|DE|DAL|DVN|DXCM|DLR|DFS|DISCA|DISCK|DISH|DG|DLTR|D|DPZ|DOV|DOW|DTE|DUK|DRE|DD|DXC|DGX|DIS|ED|ETFC|EMN|ETN|EBAY|ECL|EIX|EW|EA|EMR|ETR|EOG|EFX|EQIX|EQR|ESS|EL|EVRG|ES|EXC|EXPE|EXPD|EXR|FANG|FFIV|FB|FAST|FRT|FDX|FIS|FITB|FE|FRC|FISV|FLT|FLIR|FLS|FMC|F|FTNT|FTV|FBHS|FOXA|FOX|FCX|FTI|GOOGL|GOOG|GLW|GPS|GRMN|GD|GE|GIS|GM|GPC|GILD|GL|GPN|GS|GWW|HRB|HAL|HBI|HIG|HAS|HCA|HSIC|HSY|HES|HPE|HLT|HFC|HOLX|HD|HON|HRL|HST|HWM|HPQ|HUM|HBAN|HII|IT|IEX|IDXX|INFO|ITW|ILMN|INCY|IR|INTC|ICE|IBM|IP|IPG|IFF|INTU|ISRG|IVZ|IPGP|IQV|IRM|JKHY|J|JBHT|JNJ|JCI|JPM|JNPR|KMX|KO|KSU|K|KEY|KEYS|KMB|KIM|KMI|KLAC|KSS|KHC|KR|LNT|LB|LHX|LH|LRCX|LW|LVS|LEG|LDOS|LEN|LLY|LNC|LIN|LYV|LKQ|LMT|L|LOW|LYB|LUV|MMM|MO|MTB|MRO|MPC|MKTX|MAR|MMC|MLM|MAS|MA|MKC|MXIM|MCD|MCK|MDT|MRK|MET|MTD|MGM|MCHP|MU|MSFT|MAA|MHK|MDLZ|MNST|MCO|MS|MOS|MSI|MSCI|MYL|NDAQ|NOV|NTAP|NFLX|NWL|NEM|NWSA|NWS|NEE|NLSN|NKE|NI|NBL|NSC|NTRS|NOC|NLOK|NCLH|NRG|NUE|NVDA|NVR|NOW|ORLY|OXY|ODFL|OMC|OKE|ORCL|OTIS|O|PEAK|PCAR|PKG|PH|PAYX|PAYC|PYPL|PNR|PBCT|PEP|PKI|PRGO|PFE|PM|PSX|PNW|PXD|PNC|PPG|PPL|PFG|PG|PGR|PLD|PRU|PEG|PSA|PHM|PVH|PWR|QRVO|QCOM|RE|RL|RJF|RTX|REG|REGN|RF|RSG|RMD|RHI|ROK|ROL|ROP|ROST|RCL|SCHW|STZ|SJM|SPGI|SBAC|SLB|STX|SEE|SRE|SHW|SPG|SWKS|SLG|SNA|SO|SWK|SBUX|STT|STE|SYK|SIVB|SYF|SNPS|SYY|T|TAP|TMUS|TROW|TTWO|TPR|TGT|TEL|TDY|TFX|TXN|TXT|TMO|TIF|TJX|TSCO|TT|TDG|TRV|TFC|TWTR|TYL|TSN|UDR|ULTA|USB|UAA|UA|UNP|UAL|UNH|UPS|URI|UHS|UNM|VFC|VLO|VAR|VTR|VRSN|VRSK|VZ|VRTX|VIAC|V|VNO|VMC|WRB|WAB|WMT|WBA|WM|WAT|WEC|WFC|WELL|WST|WDC|WU|WRK|WY|WHR|WMB|WLTW|WYNN|XRAY|XOM|XEL|XRX|XLNX|XYL|YUM|ZBRA|ZBH|ZION|ZTS";
                var dowSymbols = "MMM|AXP|AMGN|AAPL|BA|CAT|CVX|CSCO|KO|DIS|DOW|GS|HD|HON|IBM|INTC|JNJ|JPM|MCD|MRK|MSFT|NKE|PG|CRM|TRV|UNH|VZ|V|WBA|WMT";
                var qqqSymbols = "AAPL|MSFT|AMZN|TSLA|FB|GOOGL|GOOG|NVDA|PYPL|ADBE|NFLX|CMCSA|INTC|PEP|CSCO|AVGO|QCOM|COST|TMUS|TXN|AMGN|CHTR|SBUX|AMD|INTU|ISRG|BKNG|MELI|MDLZ|MU|AMAT|FISV|ADP|JD|GILD|ZM|LRCX|CSX|ATVI|ADSK|VRTX|MRNA|BIDU|ADI|ILMN|REGN|LULU|MNST|DOCU|CTSH|NXPI|KDP|PDD|WDAY|KHC|MAR|EXC|ROST|ALGN|IDXX|EA|KLAC|BIIB|SNPS|EBAY|XLNX|CTAS|ASML|CDNS|WBA|XEL|MCHP|ALXN|SGEN|PAYX|DXCM|ORLY|VRSK|NTES|ANSS|PCAR|CPRT|FAST|SIRI|DLTR|SPLK|VRSN|SWKS|CERN|MXIM|TTWO|INCY|CDW|TCOM|EXPE|CHKP|BMRN|CTXS|ULTA|LBTYK|FOXA|FOX|LBTYA|QQQ";

				//var listOfSymbols = new List<string> { "AAPL", "AMD" };
				var listOfSymbols = spxSymbols.Split('|').ToList();
				listOfSymbols.AddRange(dowSymbols.Split('|').ToList());
				listOfSymbols.AddRange(qqqSymbols.Split('|').ToList());
				var listOfDistinctSymbols = listOfSymbols.Distinct();

                var client = new HttpClient();
                cosmosClient = new CosmosClient("https://optionvolumes.documents.azure.com:443/", "cYWKVAv33322EDySCxiMzVugsdge3Wc9Vh6eSOKTHK3p1WEyD8rRJLAmaRhzVbnmm9u9bnFRIf9DFIzo2CguRQ==", new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
                database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
                container = await database.CreateContainerIfNotExistsAsync(containerId, "/USymbol", 400);


                foreach (var symbol in listOfDistinctSymbols)
                {
                    try
                    {
                        var response = client.GetAsync(string.Format("https://api.tdameritrade.com/v1/marketdata/chains?apikey=6ANM3TCMXETNKQLNHSCYJCLYNGKHUHLS%40AMER.OAUTHAP&symbol={0}&contractType=ALL&strikeCount=100", symbol));
                        if(response.Result.StatusCode == HttpStatusCode.TooManyRequests)
						{
                            Thread.Sleep(60000);
                            response = client.GetAsync(string.Format("https://api.tdameritrade.com/v1/marketdata/chains?apikey=6ANM3TCMXETNKQLNHSCYJCLYNGKHUHLS%40AMER.OAUTHAP&symbol={0}&contractType=ALL&strikeCount=100", symbol));
                        }
                        var responseString = response.Result.Content.ReadAsStringAsync().Result;

                        //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                        var data = JsonConvert.DeserializeObject<Security>(responseString);

                        foreach (var expiration in data.Puts)
                        {
                            foreach (var strike in expiration.Value)
                            {
                                foreach (var contract in strike.Value)
                                {
                                    var doc = new UOActivityDocument
                                    {
                                        USymbol = data.USymbol,
                                        OSymbol = contract.OSymbol,
                                        Id = contract.OSymbol,
                                        Description = contract.Description,
                                        Strike = strike.Key,
                                        StringExpirationDate = expiration.Key,
                                        ExpirationDate = contract.ExpirationDate,
                                        TotalVolume = contract.TotalVolume,
                                        OpenInterest = contract.OpenInterest,
                                        Delta = contract.Delta,
                                        Gamma = contract.Gamma,
                                        Rho = contract.Rho,
                                        Theta = contract.Theta,
                                        Vega = contract.Vega,
                                        ContractType = "Put"
                                    };

                                    docs.Add(doc);
                                }
                            }
                        }
                        foreach (var expiration in data.Calls)
                        {
                            foreach (var strike in expiration.Value)
                            {
                                foreach (var contract in strike.Value)
                                {
                                    var doc = new UOActivityDocument
                                    {
                                        USymbol = data.USymbol,
                                        OSymbol = contract.OSymbol,
                                        Id = contract.OSymbol,
                                        Description = contract.Description,
                                        Strike = strike.Key,
                                        StringExpirationDate = expiration.Key,
                                        ExpirationDate = contract.ExpirationDate,
                                        TotalVolume = contract.TotalVolume,
                                        OpenInterest = contract.OpenInterest,
                                        Delta = contract.Delta,
                                        Gamma = contract.Gamma,
                                        Rho = contract.Rho,
                                        Theta = contract.Theta,
                                        Vega = contract.Vega,
                                        ContractType = "Call"
                                    };

                                    docs.Add(doc);
                                }
                            }
                        }

                        foreach (var item in docs.Where(x => x.OpenInterest >= 20000 || x.TotalVolume >= 10000))
                        {

                            try
                            {
                                // Read the item to see if it exists.  
                                ItemResponse<UOActivityDocument> itemDocResponse = await container.ReadItemAsync<UOActivityDocument>(item.OSymbol, new PartitionKey(item.USymbol));
                                
                                var previousItem = itemDocResponse.Resource;
                                Console.WriteLine("Item in database with id: {0} already exists. Trying upsert\n", item.OSymbol);
                                log.LogTrace("Item in database with id: {0} already exists. Trying upsert\n", item.OSymbol);

                                item.PreviousRunDateTime = previousItem.LastRunDateTime;
                                item.LastRunDateTime = DateTime.UtcNow;
                                item.VolumeAtPreviousRun = previousItem.TotalVolume;
                                item.VolumeAtLastRun = item.TotalVolume;
                                item.VolumeDelta = item.VolumeAtLastRun - item.VolumeAtPreviousRun;

                                if (item.OpenInterest != previousItem.OpenInterest)
                                {
                                    item.PreviousOpenInterest3 = previousItem.PreviousOpenInterest2;
                                    item.PreviousOpenInterest2 = previousItem.PreviousOpenInterest1;
                                    item.PreviousOpenInterest1 = previousItem.PreviousOpenInterest;
                                    item.PreviousOpenInterest = previousItem.OpenInterest;
                                }

                                ItemResponse<UOActivityDocument> itemDocUpsertResponse = await container.UpsertItemAsync(item, new PartitionKey(item.USymbol));

                                Console.WriteLine("{0} upsert successful.  {1} Upsert Operation consumed {2} RUs.\n", item.OSymbol, itemDocUpsertResponse.Resource.Id, itemDocUpsertResponse.RequestCharge);
                                log.LogTrace("{0} upsert successful.  {1} Operation consumed {2} RUs.\n", item.OSymbol, itemDocUpsertResponse.Resource.Id, itemDocUpsertResponse.RequestCharge);
                            }
                            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                            {
                                // Create an item in the container representing the option activity. Note we provide the value of the partition key for this item, which is underlying symbol
                                ItemResponse<UOActivityDocument> itemDocResponse = await container.CreateItemAsync(item, new PartitionKey(item.USymbol));

                                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemDocResponse.Resource.Id, itemDocResponse.RequestCharge);
                                log.LogTrace("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemDocResponse.Resource.Id, itemDocResponse.RequestCharge);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception Occured: {0} \n", ex.Message);
                                log.LogTrace("Exception Occured: {0} \n", ex.Message);

                            }
                        }
                        docs.Clear();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception Occured getting data for : {0} \n {1}", symbol, ex.Message);
                        log.LogTrace("Exception Occured getting data for : {0} \n {1}", symbol, ex.Message);
                    }
                }
                return new OkObjectResult("Processing Complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occured: {0} \n", ex.Message);
                log.LogTrace("Exception Occured: {0} \n", ex.Message);
                return new OkObjectResult("Processing Exception Occured");
            }
        }
    }
}
