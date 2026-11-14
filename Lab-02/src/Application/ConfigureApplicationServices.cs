using System.Reflection;
using Application.COmmon.Behaviours;
using ;
using ;
using ;
namespace ;
public static class ConfigureApplicationServices
{
public static void AddApplicationServices(this IServiceCollection services)
{
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
services.AddMediatR(configuration =>
configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
services.AddTransient(typeof(IPipelineBehavior<,>),
typeof(ValidationBehaviour<,>));
}
}