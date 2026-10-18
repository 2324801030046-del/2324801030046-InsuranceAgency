using FluentValidation;
using MediatR;
using global::PricingService.Api.Commands;
using global::PricingService.Api.Commands.Dto;
using PricingService.Domain;

namespace PricingService.Commands;

public class CalculatePriceHandler : IRequestHandler<CalculatePriceCommand, CalculatePriceResult>
{
    private readonly IDataStore dataStore;

    public CalculatePriceHandler(IDataStore dataStore)
    {
        this.dataStore = dataStore;
    }

    public async Task<CalculatePriceResult> Handle(CalculatePriceCommand cmd, CancellationToken cancellationToken)
    {
        new CalculatePriceCommandValidator().ValidateAndThrow(cmd);

        var tariff = await dataStore.Tariffs[cmd.ProductCode];
        if (tariff == null)
        {
            throw new ArgumentException($"Tariff with code '{cmd.ProductCode}' does not exist.");
        }

        var calculation = new Calculation(
            cmd.ProductCode,
            cmd.PolicyFrom,
            cmd.PolicyTo,
            cmd.SelectedCovers,
            cmd.Answers.ToDictionary(a => a.QuestionCode, a => a.GetAnswer())
        );

        var result = tariff.CalculatePrice(calculation);

        return new CalculatePriceResult
        {
            TotalPrice = result.TotalPremium,
            CoverPrices = result.Covers.ToDictionary(c => c.Key, c => c.Value.Price)
        };
    }
}
