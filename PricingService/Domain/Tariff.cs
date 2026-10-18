namespace PricingService.Domain;

public class Tariff
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public BasePremiumCalculationRuleList BasePremiumRules { get; set; } = new();
    public DiscountMarkupRuleList DiscountMarkupRules { get; set; } = new();

    public Tariff() { }

    public Tariff(string code)
    {
        Code = code;
    }

    public Calculation CalculatePrice(Calculation calculation)
    {
        CalcBasePrices(calculation);
        ApplyDiscounts(calculation);
        return calculation;
    }

    private void CalcBasePrices(Calculation calculation)
    {
        foreach (var cover in calculation.SelectedCovers)
        {
            var rule = BasePremiumRules.Find(cover, calculation);
            if (rule != null)
            {
                var price = rule.Calculate(calculation);
                calculation.UpdateCoverPrice(cover, price);
            }
        }
    }

    private void ApplyDiscounts(Calculation calculation)
    {
        var total = calculation.Covers.Values.Sum(c => c.Price);
        var markup = 1.0M;
        foreach (var rule in DiscountMarkupRules.Rules)
        {
            if (rule.Matches(calculation))
            {
                markup *= rule.PercentMarkup;
            }
        }
        calculation.TotalPremium = Math.Round(total * markup, 2);
    }
}
