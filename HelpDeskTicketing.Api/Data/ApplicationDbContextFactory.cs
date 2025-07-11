using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; 
using System.IO; 

namespace HelpDeskTicketing.Api.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Budujemy konfigurację w sposób bardziej "odporny"
            IConfigurationRoot configuration = new ConfigurationBuilder()
                // Ustawiamy ścieżkę bazową na folder, w którym znajduje się plik projektu API
                .SetBasePath(Directory.GetCurrentDirectory())
                // Dodajemy appsettings.json, jawnie wskazując, że jest opcjonalny i ma być przeładowywany
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Odczytujemy connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Sprawdzamy, czy connection string został w ogóle znaleziony
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}