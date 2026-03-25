namespace CourierBackend.Data
{
    using Microsoft.EntityFrameworkCore;

    public static class DataExtensions
    {
        public static void MigrateDb(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CourierContext>();
            dbContext.Database.Migrate();
        }

        public static void DBStoreConnection(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Courier.db";

            // DbContext has a Scoped service lifetime because:
            // 1. It ensures that a new instance of DbContext is created per request
            // 2. DB connections are a limited and expensive resource
            // 3. DbContext is not thread-safe. Scoped to avoid concurrency issues
            // 4. Makes it easier to manage transactions and ensure data consistency
            // 5. Reusing a DbContext instance can lead to increased memory usage

            //builder.Services.AddSqlite<CourierContext>(connectionString);
            builder.Services.AddDbContext<CourierContext>(options => options.UseSqlite(connectionString));
        }
    }
}