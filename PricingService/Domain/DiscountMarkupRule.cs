using DynamicExpresso;

namespace PricingService.Domain;

public class DiscountMarkupRule
{
    public string Condition { get; set; }
    public decimal PercentMarkup { get; set; }

    public DiscountMarkupRule() { }

    public DiscountMarkupRule(string condition, decimal percentMarkup)
    {
        Condition = condition;
        PercentMarkup = percentMarkup;
    }

    public bool Matches(Calculation calculation)
    {
        if (string.IsNullOrWhiteSpace(Condition)) return true;
        var interpreter = new Interpreter();
        foreach (var param in calculation.Subject)
        {
            interpreter.SetVariable(param.Key, param.Value, param.Value?.GetType() ?? typeof(object));
        }
        var res = interpreter.Eval(Condition);
        return res is bool b && b;
    }
}

public class DiscountMarkupRuleList
{
    public List<DiscountMarkupRule> Rules { get; set; } = new();

    public void AddPercentMarkup(string condition, decimal percentMarkup)
    {
        Rules.Add(new DiscountMarkupRule(condition, percentMarkup));
    }
}
