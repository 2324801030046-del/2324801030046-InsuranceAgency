using Microsoft.Extensions.DependencyInjection;

namespace PaymentService.Implementation;

public static class PaymentServiceRegistration
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PaymentServiceRegistration).Assembly));
        return services;
    }
}
