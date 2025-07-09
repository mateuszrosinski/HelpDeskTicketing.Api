using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HelpDeskTicketing.Api.Data
{
    // Ta klasa jest używana tylko przez narzędzia deweloperskie (np. do tworzenia migracji)
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Budujemy konfigurację, aby odczytać connection string z appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Tworzymy opcje dla DbContext, jawnie podając connection string
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);

            // Zwracamy nową instancję DbContext z tymi opcjami
            return new ApplicationDbContext(builder.Options);
        }
    }
}