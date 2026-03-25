using CourierBackend.Data;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// database context
builder.DBStoreConnection();

// validation service
builder.Services.AddValidation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["UserSettings:AppName"],
        Description = builder.Configuration["UserSettings:AppDescription"],
        Version = builder.Configuration["UserSettings:APIVersion"]
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    });
}

app.MapGet("/", () => "Hello World!");

app.Run();
