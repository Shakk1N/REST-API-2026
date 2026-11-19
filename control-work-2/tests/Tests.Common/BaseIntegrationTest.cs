using System.Net.Http.Headers;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common.Auth;
using Tests.Common.Extensions;
using Xunit;

namespace Tests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory>
{
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var clientOptions = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        Client = factory
            .WithWebHostBuilderMock()
            .CreateClient(clientOptions);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
    }

    protected ApplicationDbContext Context { get; }
    protected HttpClient Client { get; }
}
