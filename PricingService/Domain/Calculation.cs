namespace PricingService.Domain;

public class Calculation
{
    public string ProductCode { get; }
    public DateTimeOffset PolicyFrom { get; }
    public DateTimeOffset PolicyTo { get; }
    public List<string> SelectedCovers { get; }
    public Dictionary<string, object> Subject { get; }
    public decimal TotalPremium { get; set; }
    public Dictionary<string, Cover> Covers { get; } = new();

    public Calculation(
        string productCode,
        DateTimeOffset policyFrom,
        DateTimeOffset policyTo,
        List<string> selectedCovers,
        Dictionary<string, object> subject)
    {
        ProductCode = productCode;
        PolicyFrom = policyFrom;
        PolicyTo = policyTo;
        SelectedCovers = selectedCovers ?? new List<string>();
        Subject = subject ?? new Dictionary<string, object>();
        foreach (var cover in SelectedCovers)
        {
            Covers[cover] = new Cover(cover, 0M);
        }
    }

    public void UpdateCoverPrice(string coverCode, decimal price)
    {
        if (Covers.ContainsKey(coverCode))
        {
            Covers[coverCode].Price = price;
        }
    }
}
