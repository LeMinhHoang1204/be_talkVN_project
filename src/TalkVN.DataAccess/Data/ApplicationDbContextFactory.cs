using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TalkVN.DataAccess.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var configPath = Path.Combine(basePath, "..", "TalkVN.WebAPI");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 👉 Thêm dòng này để kiểm tra connection string
            Console.WriteLine(">>> CONNECTION STRING: " + connectionString);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 39)),
                options => options.EnableRetryOnFailure()
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
