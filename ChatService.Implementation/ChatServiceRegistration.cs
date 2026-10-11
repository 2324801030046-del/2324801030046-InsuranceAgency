using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Implementation;

public static class ChatServiceRegistration
{
    public static IServiceCollection AddChatServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ChatServiceRegistration).Assembly));
        return services;
    }
}
