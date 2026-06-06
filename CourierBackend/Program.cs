using CourierBackend.Data;
using CourierBackend.Helpers;
using Microsoft.OpenApi;
using CourierBackend.Data.Validators.Admin;
using CourierBackend.Services;
using CourierBackend.Data.Repositories;
using CourierBackend.Data.Repositories.Interfaces;
using CourierBackend.Services.Model.Interfaces;
using CourierBackend.Services.Model;
using System.Text.Json.Serialization;
using CourierBackend.Middlewares;
using CourierBackend.Services.Email;
using CourierBackend.Services.Email.Interfaces;
using CourierBackend.Configurations.Services;
using CourierBackend.Services.Auth.Interfaces;
using CourierBackend.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
string? apiVersion = builder.Configuration["UserSettings:APIVersion"];

// database context
builder.DBStoreConnection();

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

// validation service
builder.Services.AddValidatorsFromAssemblyContaining<CreateValidator>();

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

// change the routing to be lowercase for better SEO and consistency
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Email service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Auth service
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtService, JwtService>();

// setup JWT authentication
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,

            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(authHeader) &&
                    authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader["Bearer ".Length..].Trim();
                }
                else
                {
                    context.Token = context.Request.Cookies["token"];
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// current admin service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentAdminService, CurrentAdminService>();

// file service
builder.Services.AddScoped<IFileService, FileService>();

// swagger options
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(apiVersion, new OpenApiInfo
    {
        Title = builder.Configuration["UserSettings:AppName"],
        Description = builder.Configuration["UserSettings:AppDescription"],
        Version = apiVersion
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your_token}"
    });
});

var app = builder.Build();

//app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", builder.Configuration["UserSettings:AppDescription"]);
        
        // Keeps JWT token after page refresh
        options.ConfigObject.PersistAuthorization = true;
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();