using DynamicExpresso;

namespace PricingService.Domain;

public class BasePremiumCalculationRule
{
    public string CoverCode { get; set; }
    public string Condition { get; set; }
    public string Formula { get; set; }

    public BasePremiumCalculationRule() { }

    public BasePremiumCalculationRule(string coverCode, string condition, string formula)
    {
        CoverCode = coverCode;
        Condition = condition;
        Formula = formula;
    }

    public bool Matches(Calculation calculation)
    {
        if (string.IsNullOrWhiteSpace(Condition)) return true;
        var interpreter = CreateInterpreter(calculation);
        var res = interpreter.Eval(Condition);
        return res is bool b && b;
    }

    public decimal Calculate(Calculation calculation)
    {
        var interpreter = CreateInterpreter(calculation);
        var result = interpreter.Eval(Formula);
        return Convert.ToDecimal(result);
    }

    private Interpreter CreateInterpreter(Calculation calculation)
    {
        var interpreter = new Interpreter();
        foreach (var param in calculation.Subject)
        {
            interpreter.SetVariable(param.Key, param.Value, param.Value?.GetType() ?? typeof(object));
        }
        return interpreter;
    }
}

public class BasePremiumCalculationRuleList
{
    public List<BasePremiumCalculationRule> Rules { get; set; } = new();

    public void AddBasePriceRule(string coverCode, string condition, string formula)
    {
        Rules.Add(new BasePremiumCalculationRule(coverCode, condition, formula));
    }

    public BasePremiumCalculationRule Find(string coverCode, Calculation calculation)
    {
        return Rules.FirstOrDefault(r => r.CoverCode == coverCode && r.Matches(calculation));
    }
}
