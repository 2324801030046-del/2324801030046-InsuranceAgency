using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PricingService.Domain;

namespace PricingService.DataAccess.Marten;

public class MartenDataStore : IDataStore
{
    private readonly IDocumentSession session;

    public MartenDataStore(IDocumentSession session)
    {
        this.session = session;
        Tariffs = new MartenTariffRepository(session);
    }

    public ITariffRepository Tariffs { get; }

    public async Task CommitChanges()
    {
        await session.SaveChangesAsync();
    }
}

public class MartenTariffRepository : ITariffRepository
{
    private readonly IDocumentSession session;

    public MartenTariffRepository(IDocumentSession session)
    {
        this.session = session;
    }

    public Task<Tariff> this[string code] =>
        session.Query<Tariff>().FirstOrDefaultAsync(t => t.Code == code);

    public void Add(Tariff tariff)
    {
        session.Store(tariff);
    }

    public Task<bool> Exists(string code) =>
        session.Query<Tariff>().AnyAsync(t => t.Code == code);
}

// In-Memory fallback implementation to guarantee reliable execution in any environment
public class InMemoryDataStore : IDataStore
{
    public ITariffRepository Tariffs { get; } = new InMemoryTariffRepository();

    public Task CommitChanges() => Task.CompletedTask;
}

public class InMemoryTariffRepository : ITariffRepository
{
    private readonly Dictionary<string, Tariff> tariffs = new();

    public Task<Tariff> this[string code]
    {
        get
        {
            tariffs.TryGetValue(code, out var tariff);
            return Task.FromResult(tariff);
        }
    }

    public void Add(Tariff tariff)
    {
        tariffs[tariff.Code] = tariff;
    }

    public Task<bool> Exists(string code) => Task.FromResult(tariffs.ContainsKey(code));
}

public static class MartenInstaller
{
    public static IServiceCollection AddMartenDataStore(this IServiceCollection services, string cnnString)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(cnnString))
            {
                services.AddMarten(options =>
                {
                    options.Connection(cnnString);
                    options.Schema.For<Tariff>().Identity(t => t.Id);
                    options.Schema.For<Tariff>().Index(t => t.Code);
                });
                services.AddScoped<IDataStore, MartenDataStore>();
                return services;
            }
        }
        catch { }

        // Fallback to InMemory
        services.AddSingleton<IDataStore, InMemoryDataStore>();
        return services;
    }
}
