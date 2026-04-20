using CourierBackend.Data;
using FluentValidation;
using Microsoft.OpenApi;
using CourierBackend.Data.Validators.Admin;
using CourierBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// database context
builder.DBStoreConnection();

// validation service
builder.Services.AddValidation();

builder.Services.AddValidatorsFromAssemblyContaining<CreateValidator>();

// query service
builder.Services.AddScoped<IQueryService, QueryService>();

// pagination service
builder.Services.AddScoped<IPaginationService, PaginationService>();

// cache service
builder.Services.AddMemoryCache();

builder.Services.AddControllers();

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Configuration["UserSettings:AppDescription"]);
    });
}

app.MapControllers();

app.Run();
