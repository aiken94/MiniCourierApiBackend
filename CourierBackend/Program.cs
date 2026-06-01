using CourierBackend.Data;
using FluentValidation;
using Microsoft.OpenApi;
using CourierBackend.Data.Validators.Admin;
using CourierBackend.Services;
using CourierBackend.Data.Repositories;
using CourierBackend.Data.Repositories.Interfaces;
using CourierBackend.Services.Model.Interfaces;
using CourierBackend.Services.Model;
using System.Text.Json.Serialization;
using CourierBackend.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// database context
builder.DBStoreConnection();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

// validation service
//builder.Services.AddValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateValidator>();

// register MVC
builder.Services.AddControllers();

// query service
builder.Services.AddScoped<IQueryService, QueryService>();

// pagination service
builder.Services.AddScoped<IPaginationService, PaginationService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IPackageService, PackageService>();

builder.Services.AddScoped<IDeliveryHistoryRepository, DeliveryHistoryRepository>();
builder.Services.AddScoped<IDeliveryHistoryService, DeliveryHistoryService>();

// cache service
builder.Services.AddMemoryCache();

// controllers already registered above via mvcBuilder

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

// change the routing to be lowercase for better SEO and consistency
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// file service
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

//app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles();

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

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services)
    {
        var assembly = typeof(T).Assembly;

        foreach (var type in assembly.ExportedTypes)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                continue;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                {
                    continue;
                }

                if (interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }

        return services;
    }
}
