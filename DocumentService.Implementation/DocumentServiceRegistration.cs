using Microsoft.Extensions.DependencyInjection;

namespace DocumentService.Implementation;

public static class DocumentServiceRegistration
{
    public static IServiceCollection AddDocumentServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DocumentServiceRegistration).Assembly));
        return services;
    }
}
