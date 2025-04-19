namespace RookieShop.ProductCatalog.Application.Entities;

public class Product
{
    public string Sku { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public decimal Price { get; set; }
    
    public Category Category { get; set; }
    
    public string ImageUrl { get; set; }
    
    public bool IsFeatured { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime UpdatedDate { get; set; }
    
    public Rating Rating { get; set; }
    
#pragma warning disable CS8618, CS9264
    public Product() {}
#pragma warning restore CS8618, CS9264
}

public class Rating
{
    public string ProductSku { get; init; }
    
    public double Score { get; private set; }
    
    public int OneCount { get; private set; }
    
    public int TwoCount { get; private set; }
    
    public int ThreeCount { get; private set; }
    
    public int FourCount { get; private set; }
    
    public int FiveCount { get; private set; }

    private int Count => OneCount + TwoCount + ThreeCount + FourCount + FiveCount;

#pragma warning disable CS8618, CS9264
    public Rating() {}
#pragma warning restore CS8618, CS9264
    
    public Rating(string productSku)
    {
        ProductSku = productSku;
        Score = 0.0;
        OneCount = 0;
        TwoCount = 0;
        ThreeCount = 0;
        FourCount = 0;
        FiveCount = 0;
    }

    public void ApplyScore(int score)
    {
        Score = (Score * Count + score) / (Count + 1);

        switch (score)
        {
            case 1:
                OneCount++;
                break;
            case 2:
                TwoCount++;
                break;
            case 3:
                ThreeCount++;
                break;
            case 4:
                FourCount++;
                break;
            case 5:
                FiveCount++;
                break;
            default:
                return;
        }
    }
}