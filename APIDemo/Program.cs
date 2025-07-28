using Azure;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ICustomerRepository, MockCustomerRepository>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/GetCustomerInfo/{id}", (
    ICustomerRepository repo,
    string id,
    DateOnly? startDate,
    DateOnly? endDate) => 
{
    var conString = builder.Configuration.GetConnectionString("BloggingDatabase") ??
     throw new InvalidOperationException("Connection string 'BloggingDatabase' not found.");

    var dataTable = new DataTable();
    try
    {
        using (var connection = new SqlConnection(conString))
        {
            string queryString = @"
            SELECT Item, Price, TypeOfPayment, Date 
            FROM Customer 
            WHERE id = @id";

            if (startDate.HasValue && endDate.HasValue)
            {
                queryString += " AND CreatedTime BETWEEN @startDate AND @endDate";
            }

            var result = connection.Query(
                queryString,
                new
                {
                    id,
                    startDate,
                    endDate
                }
            );

            return JsonConvert.SerializeObject(result);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return null;
    }

});
app.MapGet("/MockGetCustomerInfo/{id}", (
    ICustomerRepository repo,
    string id,
    DateOnly? startDate,
    DateOnly? endDate) =>
{
    string JSONresult;
    if (startDate.HasValue && endDate.HasValue)
    {
        JSONresult = JsonConvert.SerializeObject(repo.GetCustomerInfo(id, startDate.Value, endDate.Value));
        return JSONresult;
    }
    else
    {
        JSONresult = JsonConvert.SerializeObject(repo.GetCustomerInfo(id));
        return JSONresult;
    }
});

app.Run();

