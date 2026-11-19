using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using Tests.Common.Extensions;
using Xunit;

namespace Tests.Common;

public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("test_exercise_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Test.json", optional: true);
            config.AddEnvironmentVariables();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveServiceByType(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveServiceByType(typeof(ApplicationDbContext));

            var connectionString = _dbContainer.GetConnectionString();
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(dataSource, b =>
                        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    )
                    .UseSnakeCaseNamingConvention()
                    .ConfigureWarnings(warnings =>
                    {
                        warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
                    })
            );
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
