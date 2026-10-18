namespace PricingService.Domain;

public class Cover
{
    public string Code { get; set; }
    public decimal Price { get; set; }

    public Cover() { }

    public Cover(string code, decimal price)
    {
        Code = code;
        Price = price;
    }
}
