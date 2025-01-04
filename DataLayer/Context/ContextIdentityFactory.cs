using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataLayer.API.Context;


namespace Accapt.DataLayer.ContextIdentoty
{
    public class ContextIdentityFactory : IDesignTimeDbContextFactory<UserMangerContext>
    {
        public UserMangerContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<UserMangerContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("LocalConnectionDB"));

            return new UserMangerContext(optionsBuilder.Options);
        }
    }
}
