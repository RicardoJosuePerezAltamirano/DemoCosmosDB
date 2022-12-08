using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// servicios cosmosDb
string Url = "https://demobasenosql.documents.azure.com:443/";
string key = "wlOS6znejADO8eyJGYzj4Ia3kqrcSUH0ENLodohRK0jdMju0inLvjWPwYfKGljHSHD8yqGWsPJvZYx3bfpEgwg==";
builder.Services.AddSingleton<CosmosClient>(o =>
{
    var client= new CosmosClient(Url, key);
    client.ClientOptions.ConnectionMode = ConnectionMode.Gateway;
    client.ClientOptions.GatewayModeMaxConnectionLimit = 100;
    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
