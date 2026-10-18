using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PricingService.Domain;

namespace PricingService.Init;

public class DataLoader
{
    private readonly IDataStore dataStore;

    public DataLoader(IDataStore dataStore)
    {
        this.dataStore = dataStore;
    }

    public async Task Seed()
    {
        if (!await dataStore.Tariffs.Exists("TRI"))
            dataStore.Tariffs.Add(DemoTariffFactory.Travel());

        if (!await dataStore.Tariffs.Exists("HSI"))
            dataStore.Tariffs.Add(DemoTariffFactory.House());

        if (!await dataStore.Tariffs.Exists("FAI"))
            dataStore.Tariffs.Add(DemoTariffFactory.Farm());

        if (!await dataStore.Tariffs.Exists("CAR"))
            dataStore.Tariffs.Add(DemoTariffFactory.Car());

        await dataStore.CommitChanges();
    }
}

public static class DataLoaderInstaller
{
    public static IServiceCollection AddPricingDemoInitializer(this IServiceCollection services)
    {
        services.AddScoped<DataLoader>();
        return services;
    }

    public static IApplicationBuilder UseInitializer(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetService<DataLoader>();
        initializer?.Seed().GetAwaiter().GetResult();
        return app;
    }
}
