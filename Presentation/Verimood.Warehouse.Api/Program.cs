using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Verimood.Warehouse.Api.Configurations;
using Verimood.Warehouse.Application;
using Verimood.Warehouse.Application.Exceptions;
using Verimood.Warehouse.Application.Exceptions.Models;
using Verimood.Warehouse.Infrastructure;
using Verimood.Warehouse.Persistence;


var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurations();

builder.Host.UseSerilog((_, serilogConfig) =>
{
    serilogConfig
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .Enrich.FromLogContext();
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPersistence();      
builder.Services.AddInfrastructure();   
builder.Services.AddApplication();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("auth", new OpenApiInfo { Title = "Auth API", Version = "v1" });
    opt.SwaggerDoc("user", new OpenApiInfo { Title = "User API", Version = "v1" });
    opt.SwaggerDoc("category", new OpenApiInfo { Title = "Category API", Version = "v1" });
    opt.SwaggerDoc("product", new OpenApiInfo { Title = "Product API", Version = "v1" });
    opt.SwaggerDoc("stock", new OpenApiInfo { Title = "Stock API", Version = "v1" });
    opt.SwaggerDoc("customer", new OpenApiInfo { Title = "Customer API", Version = "v1" });


    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    opt.MapType<CustomExceptionModel>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = {
                { "StatusCode", new OpenApiSchema { Type = "integer" } },
                { "ErrorMessages", new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string" } } }
        }
    });
});

var app = builder.Build();

app.ConfigureExceptionHandling();
app.UseHttpsRedirection();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth API");
        opt.SwaggerEndpoint("/swagger/user/swagger.json", "User API");
        opt.SwaggerEndpoint("/swagger/category/swagger.json", "Category API");
        opt.SwaggerEndpoint("/swagger/product/swagger.json", "Product API");
        opt.SwaggerEndpoint("/swagger/stock/swagger.json", "Stock API");
        opt.SwaggerEndpoint("/swagger/customer/swagger.json", "Customer API");
        opt.RoutePrefix = string.Empty;
    });
}

app.UsePersistenceLayer();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
 
app.Run();
