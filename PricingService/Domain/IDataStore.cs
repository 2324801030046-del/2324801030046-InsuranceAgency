namespace PricingService.Domain;

public interface IDataStore
{
    ITariffRepository Tariffs { get; }
    Task CommitChanges();
}

public interface ITariffRepository
{
    Task<Tariff> this[string code] { get; }
    void Add(Tariff tariff);
    Task<bool> Exists(string code);
}
