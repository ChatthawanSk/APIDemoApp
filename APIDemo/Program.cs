using Azure;
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
            if (startDate.HasValue && endDate.HasValue)
            {
                var queryString = @"
                SELECT Item, Price, TypeOfPayment, Date FROM Customer
                WHERE id = @id
                AND CreatedTime BETWEEN @startDate AND @endDate";

                using SqlCommand command = new(queryString, connection);
                command.Parameters.Add("@id", SqlDbType.Char, 10).Value = id;
                command.Parameters.Add("@startDate", SqlDbType.Date).Value = startDate;
                command.Parameters.Add("@endDate", SqlDbType.Date).Value = endDate;

                connection.Open();
                using var dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
            }

            else
            {
                var queryString = @"SELECT Item, Price, TypeOfPayment, Date FROM Customer
                                    WHERE id = @id";
                SqlCommand command = new(queryString, connection);
                command.Parameters.Add("@id", SqlDbType.Char, 10).Value = id;

                connection.Open();
                using var dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
                
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    return JsonConvert.SerializeObject(dataTable);
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

