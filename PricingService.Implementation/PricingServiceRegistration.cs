using Microsoft.Extensions.DependencyInjection;

namespace PricingService.Implementation;

public static class PricingServiceRegistration
{
    public static IServiceCollection AddPricingServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PricingServiceRegistration).Assembly));
        return services;
    }
}
