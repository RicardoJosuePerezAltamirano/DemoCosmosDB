// See https://aka.ms/new-console-template for more information
using ConsoleCosmosDB;
using Microsoft.Azure.Cosmos;

Console.WriteLine("Hello, World!");
string Url = "https://demobasenosql.documents.azure.com:443/";
string key = "wlOS6znejADO8eyJGYzj4Ia3kqrcSUH0ENLodohRK0jdMju0inLvjWPwYfKGljHSHD8yqGWsPJvZYx3bfpEgwg==";
var cosmosClient = new CosmosClient(Url, key);
var dbContainer = cosmosClient.GetDatabase("testDataBase").GetContainer("Locations");
var products = new Product()
{
    id = "1558",
    LocationId = new Guid().ToString(),
    name = "teclado",
    price = 663.3M
};

var product2 = new
{
    id = "15589213",
    LocationId = new Guid(),
    name = "cable hdm",
    price = 63.3M,
    data = new
    {
        color = "negro",
        largo = 99M
    },
    description = "cable marca steren de prueba "

};
// tipado
//var response=await dbContainer.CreateItemAsync<Product>(product, new PartitionKey(products.LocationId.ToString()));
// sin tipo
//var response = await dbContainer.CreateItemAsync(product2, new PartitionKey(product2.LocationId.ToString()));
//Console.WriteLine($"Status: {response.StatusCode} {response.RequestCharge}");

var query = new QueryDefinition("SELECT * FROM Locations as c where c.id <> ''");
//using (var obj = dbContainer.GetItemQueryIterator<Product>(query))// tipado
using (var obj = dbContainer.GetItemQueryIterator<dynamic>(query))
{
    while (obj.HasMoreResults)
    {
        var item = await obj.ReadNextAsync();
        foreach (var i in item)
        {
            string color = string.Empty;
            string descripcion = string.Empty;
            decimal largo = 0;
            try
            {
                // hacer un try para cada uno 
                color = i.data.color;
                
            }
            catch(Exception ex)
            {
                color = string.Empty;
                
                
                Console.WriteLine($"{ex.Message}");
            }
            try
             {
                descripcion = i.description;
                
            }
            catch(Exception ex)
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
            Console.WriteLine($"registro :{i.id} {i.name} {i.price} {i.LocationId} color: {color} description:{descripcion} largo:{largo} ");

        }
    }
}
