using Similarity.Application.Helpers;
using System;
using System.Text.Json;

public class SimilarityService(AppDbContext db) 
{
    private static float Cosine(float[] a, float[] b)
    {
        double dot = 0, na = 0, nb = 0;
        for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
        if (na == 0 || nb == 0) return 0f;
        return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb)));
    }

    private float CategorySimilarity(int c1, int c2) => c1 == c2 ? 1f : 0f;

    private float PriceSimilarity(decimal p1, decimal p2)
    {
        if (p1 == 0 && p2 == 0) return 1f;
        var diff = Math.Abs((double)(p1 - p2)) / Math.Max((double)p1, (double)p2);
        return (float)Math.Max(0, 1 - diff);
    }

    public List<OfferDto> GetSimilar(int id, int take = 5, bool allowCrossType = true)
    {
        // 1️⃣ Tanlangan offer
        var target = db.Offers.FirstOrDefault(o => o.Id == id);
        if (target == null) return new List<OfferDto>();
        var targetVec = JsonSerializer.Deserialize<float[]>(target.VectorJson);

        // 2️⃣ Subset: category filter va type filter
        var subset = db.Offers
                        .Where(o => o.Id != id)
                        .Where(o => allowCrossType || o.Type == target.Type)
                        .Where(o => (double)Math.Abs(o.Price - target.Price) /
           (o.Price > target.Price ? (double)o.Price : (double)target.Price) < 0.5).ToList();

        var results = new List<OfferDto>();
        foreach (var offer in subset)
        {
            var vec = JsonSerializer.Deserialize<float[]>(offer.VectorJson);
            var maxLen = Math.Max(targetVec.Length, vec.Length);
            var v1 = PadVector(targetVec, maxLen);
            var v2 = PadVector(vec, maxLen);
            var textSim = Cosine(v1, v2);
            var catSim = CategorySimilarity(target.CategoryId, offer.CategoryId);
            var priceSim = PriceSimilarity(target.Price, offer.Price);

            var final = 0.4f * textSim + 0.5f * catSim + 0.1f * priceSim;
            results.Add((offer.MapToDto(final)));
        }

        return results.OrderByDescending(r => r.Score).Take(take).ToList();
    }

    private static float[] PadVector(float[] vec, int length)
    {
        if (vec.Length >= length) return vec;
        var padded = new float[length];
        Array.Copy(vec, padded, vec.Length);
        return padded;
    }

    public async Task<List<OfferDto>> SearchAsync(string search, int take)
    {
        var queryVec = VectorHelper.ComputeVector(search);

        // 2️⃣ DBdan subsetni o'qi (masalan, type filter yoki category filter qo'shish mumkin)
        var offers = db.Offers
                        .Where(o => o.VectorJson != null)
                        .AsEnumerable() // client-side, vector bilan ishlash uchun
                        .ToList();

        // 3️⃣ Cosine similarity hisoblash
        var results = offers.Select(o =>
        {
            var vec = JsonSerializer.Deserialize<float[]>(o.VectorJson);
            var maxLen = Math.Max(queryVec.Length, vec.Length);
            var v1 = PadVector(queryVec, maxLen);
            var v2 = PadVector(vec, maxLen);
            var textSim = Cosine(v1, v2);
            return o.MapToDto(textSim);
        })
        .OrderByDescending(r => r.Score)
        .Take(take)
        .ToList();

        return results;
    }
}