using AutenticacaoJWT.Infra.Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutenticacaoJWT.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<ContextApp>)
                        || d.ServiceType == typeof(ContextApp))
                    .ToList();

                foreach (var descriptor in descriptors)
                    services.Remove(descriptor);

                services.AddDbContext<ContextApp>(options =>
                    options.UseInMemoryDatabase("AutenticacaoJWT_Tests"));
            });
        }
    }
}
