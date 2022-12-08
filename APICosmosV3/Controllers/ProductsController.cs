using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//https://learn.microsoft.com/en-us/dotnet/core/runtime-config/
// https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector
//https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/best-practice-dotnet
//https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/performance-tips-query-sdk?tabs=v3&pivots=programming-language-csharp
//https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/find-request-unit-charge?tabs=dotnetv2
namespace APICosmosV3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CosmosClient cosmosClient;

        public ProductsController(CosmosClient Client)
        {
            cosmosClient = Client;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<IEnumerable<Products>> Get()
        {
            var dbContainer = cosmosClient.GetDatabase("testDataBase").GetContainer("Locations");
            var query = new QueryDefinition("SELECT * FROM Locations as c where c.id <> ''");
            //using (var obj = dbContainer.GetItemQueryIterator<Product>(query))// tipado
            List<Products> Response = new List<Products>();
            using (var obj = dbContainer.GetItemQueryIterator<dynamic>(query))
            {
                
                while (obj.HasMoreResults)
                {
                    var item = await obj.ReadNextAsync();
                    Console.WriteLine($"DTU carga item {item.RequestCharge}"); 
                    foreach (var i in item)
                    {
                        Products ToAdd = new Products();
                        string color = string.Empty;
                        string descripcion = string.Empty;
                        decimal largo = 0;
                        try
                        {
                            // hacer un try para cada uno 
                            color = i.data.color;


                        }
                        catch (Exception ex)
                        {
                            color = string.Empty;


                            Console.WriteLine($"{ex.Message}");
                        }
                        try
                        {
                            descripcion = i.description;

                        }
                        catch (Exception ex)
                        {
                            descripcion = string.Empty;

                        }
                        try
                        {
                            largo = i.data.largo;
                        }
                        catch (Exception es)
                        {
                            largo = 0;

                        }
                        ToAdd.id= i.id;
                        ToAdd.LocationId = i.locationId;
                        ToAdd.price = i.price;
                        ToAdd.name = i.name;
                        
                        Console.WriteLine($"registro :{i.id} {i.name} {i.price} {i.LocationId} color: {color} description:{descripcion} largo:{largo} ");
                        Response.Add(ToAdd);
                    }
                   
                }
              
            }
            return Response;
        }

        [HttpGet("cache")]
        public async Task<IEnumerable<Products>> GetWithCache()
        {
            var dbContainer = cosmosClient.GetDatabase("testDataBase").GetContainer("Locations");
            var query = new QueryDefinition("SELECT * FROM Locations as c where c.id <> ''");
            var QueryResultSetIterator = dbContainer.GetItemQueryIterator<Products>(query,
                requestOptions: new QueryRequestOptions
                {
                    ConsistencyLevel = ConsistencyLevel.Eventual,
                    DedicatedGatewayRequestOptions = new DedicatedGatewayRequestOptions
                    {
                        MaxIntegratedCacheStaleness = TimeSpan.FromMinutes(10)
                    }
                   
                });
            //using (var obj = dbContainer.GetItemQueryIterator<Product>(query))// tipado
            List<Products> Response = new List<Products>();
            //using (var obj = dbContainer.GetItemQueryIterator<dynamic>(query))
            //{
            //    
            //}
            while(QueryResultSetIterator.HasMoreResults)
            {

                //while (obj.HasMoreResults)
                var item = await QueryResultSetIterator.ReadNextAsync();
                Console.WriteLine($"DTU carga item {item.RequestCharge} usando cache, debe ser cero, checar por que");
               
                    //var item = await obj.ReadNextAsync();
                    
                foreach (var auxitem in item.Resource)
                {


                    Response.Add(auxitem);
                }
            }
            return Response;
        }

        [HttpGet("stream")]
        public async Task<IEnumerable<Products>> GetWithStream()
        {
            var dbContainer = cosmosClient.GetDatabase("testDataBase").GetContainer("Locations");
            var query = new QueryDefinition("SELECT * FROM Locations as c where c.id <> ''");
            var QueryResultSetIterator = dbContainer.GetItemQueryStreamIterator(query,
                requestOptions: new QueryRequestOptions
                {
                    //ConsistencyLevel = ConsistencyLevel.Eventual,
                    //DedicatedGatewayRequestOptions = new DedicatedGatewayRequestOptions
                    //{
                    //    MaxIntegratedCacheStaleness = TimeSpan.FromMinutes(10)
                    //}
                   // PartitionKey = new PartitionKey("valor")

                }) ;
            //using (var obj = dbContainer.GetItemQueryIterator<Product>(query))// tipado
            List<Products> Response = new List<Products>();
            //using (var obj = dbContainer.GetItemQueryIterator<dynamic>(query))
            //{
            //    
            //}
            while (QueryResultSetIterator.HasMoreResults)
            {
                using(ResponseMessage response= await QueryResultSetIterator.ReadNextAsync())
                {
                    Console.WriteLine($"RU carga stream {response.Headers.RequestCharge}");
                    using (StreamReader sr = new StreamReader(response.Content))
                    {
                        
                        using (JsonTextReader jtr = new JsonTextReader(sr))
                        {
                            JObject result = JObject.Load(jtr);
                            var data = result.GetValue("Documents");
                            List<Products> auxitem = data.ToObject<List<Products>>();
                            Response.AddRange(auxitem);
                        }
                    }

                }
                //while (obj.HasMoreResults)
                //var item = await QueryResultSetIterator.ReadNextAsync();
                

                //var item = await obj.ReadNextAsync();

                //foreach (var auxitem in item.Resource)
                //{
                //
                //
                //    
                //}
            }
            return Response;
        }


        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post(Products value)
        {
            var dbContainer = cosmosClient.GetDatabase("testDataBase").GetContainer("Locations");
            var response = await dbContainer
                .CreateItemAsync<Products>(value, new PartitionKey(value.LocationId.ToString()),
                new ItemRequestOptions()
                {
                    EnableContentResponseOnWrite = false
                });
            Console.WriteLine($"RU al agregar {response.RequestCharge}");
                return Ok(response);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
