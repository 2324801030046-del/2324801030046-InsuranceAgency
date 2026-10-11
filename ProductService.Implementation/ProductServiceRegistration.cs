using Microsoft.Extensions.DependencyInjection;

namespace ProductService.Implementation;

public static class ProductServiceRegistration
{
    public static IServiceCollection AddProductServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductServiceRegistration).Assembly));
        return services;
    }
}
