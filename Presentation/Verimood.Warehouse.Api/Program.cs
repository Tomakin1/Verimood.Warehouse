using Verimood.Warehouse.Application;
using Verimood.Warehouse.Application.Exceptions;
using Verimood.Warehouse.Persistence;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddPersistence();

var app = builder.Build();

app.ConfigureExceptionHandling();
app.UseHttpsRedirection();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UsePersistenceLayer();
app.UseAuthorization();

app.MapControllers();

app.Run();
