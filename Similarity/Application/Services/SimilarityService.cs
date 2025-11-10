using Microsoft.ML;

public class SimilarityService
{
    private readonly MLContext _ml = new MLContext();
    private List<Offer> offers = new List<Offer>();
    private List<float[]> _vectors = new List<float[]>();

    public void BuildIndex(List<Offer> offers)
    {
        this.offers = offers;
        var docs = offers.Select(i => new TextData { Text = (i.Name + " " + i.Description).ToLowerInvariant() }).ToList();
        var data = _ml.Data.LoadFromEnumerable(docs);
        var pipeline = _ml.Transforms.Text.FeaturizeText("Features", nameof(TextData.Text));
        var model = pipeline.Fit(data);
        var transformed = model.Transform(data);
        var features = _ml.Data.CreateEnumerable<TextFeatures>(transformed, reuseRowObject: false).ToList();
        _vectors = features.Select(f => f.Features).ToList();
    }

    private static float Cosine(float[] a, float[] b)
    {
        double dot = 0; double na = 0; double nb = 0;
        for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
        if (na == 0 || nb == 0) return 0f;
        return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb)));
    }

    private float CategorySimilarity(int c1, int c2) => c1 == c2 ? 1f : 0f;

    private float PriceSimilarity(decimal p1, decimal p2)
    {
        if (p1 == 0 && p2 == 0) return 1f;
        var diff = Math.Abs((double)(p1 - p2)) / Math.Max((double)p1, (double)p2);
        return (float)Math.Max(0, 1 - diff); // 0..1
    }

    public List<(Offer Item, float Score)> GetSimilar(int id, int take = 5, bool allowCrossType = true)
    {
        var idx = offers.FindIndex(i => i.Id == id);
        if (idx == -1) return new List<(Offer, float)>();

        var target = offers[idx];
        var targetVec = _vectors[idx];

        var results = new List<(Offer, float)>();
        for (int i = 0; i < offers.Count; i++)
        {
            if (offers[i].Id == id) continue;
            if (!allowCrossType && offers[i].Type != target.Type) continue;

            var textSimilarity = Cosine(targetVec, _vectors[i]);
            var categorySimilarity = CategorySimilarity(target.CategoryId, offers[i].CategoryId);
            var priceSimilarity = PriceSimilarity(target.Price, offers[i].Price);

            var final = 0.4f * textSimilarity + 0.5f * categorySimilarity + 0.1f * priceSimilarity;
            results.Add((offers[i], final));
        }

        return results.OrderByDescending(r => r.Item2).Take(take).ToList();
    }
}