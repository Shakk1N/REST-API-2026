using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common.Auth;

namespace Tests.Common.Extensions;

public static class WebHostExtensions
{
    public static WebApplicationFactory<TEntryPoint> WithWebHostBuilderMock<TEntryPoint>(
        this WebApplicationFactory<TEntryPoint> factory
    ) where TEntryPoint : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: TestAuthHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.AuthenticationScheme,
                        options => { }
                    );
            });
        });
    }
}
