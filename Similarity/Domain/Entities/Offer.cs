using Microsoft.ML.Data;

public class Offer
{
    public int Id { get; set; }
    public OfferType Type { get; set; } // "Product" or "Service"
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }

    // serialized vector
    public string VectorJson { get; set; }

    public OfferDto MapToDto(float score)
    {
        return new OfferDto
        {
            Id = this.Id,
            Type = this.Type,
            Name = this.Name,
            Description = this.Description,
            CategoryId = this.CategoryId,
            Price = this.Price,
            Score = score
        };
    }
}

public class OfferDto
{
    public int Id { get; set; }
    public OfferType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public float Score { get; set; }
}

public enum OfferType { Product, Service }

public class TextData { public string Text { get; set; } }
public class TextFeatures { [VectorType] public float[] Features { get; set; } }