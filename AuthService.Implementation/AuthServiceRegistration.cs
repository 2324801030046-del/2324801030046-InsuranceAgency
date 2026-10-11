using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Implementation;

public static class AuthServiceRegistration
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AuthServiceRegistration).Assembly));
        return services;
    }
}
