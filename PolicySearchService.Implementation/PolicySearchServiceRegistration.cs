using Microsoft.Extensions.DependencyInjection;

namespace PolicySearchService.Implementation;

public static class PolicySearchServiceRegistration
{
    public static IServiceCollection AddPolicySearchServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PolicySearchServiceRegistration).Assembly));
        return services;
    }
}
