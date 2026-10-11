using Microsoft.Extensions.DependencyInjection;

namespace PolicyService.Implementation;

public static class PolicyServiceRegistration
{
    public static IServiceCollection AddPolicyServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PolicyServiceRegistration).Assembly));
        return services;
    }
}
