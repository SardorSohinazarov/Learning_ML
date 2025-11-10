using Microsoft.ML;
using System.Text.Json;

namespace Similarity.Application.Helpers
{
    public static class VectorHelper
    {
        private static readonly MLContext _ml = new MLContext();

        public static float[] ComputeVector(string text)
        {
            var data = new List<TextData> { new TextData { Text = text.ToLowerInvariant() } };
            var dv = _ml.Data.LoadFromEnumerable(data);

            var pipeline = _ml.Transforms.Text.FeaturizeText("Features", nameof(TextData.Text));
            var model = pipeline.Fit(dv);
            var transformed = model.Transform(dv);

            var features = _ml.Data.CreateEnumerable<TextFeatures>(transformed, reuseRowObject: false).First();
            return features.Features;
        }

        public static void UpdateAllOfferVectors(AppDbContext db)
        {
            var offers = db.Offers.ToList();
            foreach (var offer in offers)
            {
                CreateVector(offer);
            }
            db.SaveChanges();
        }

        public static void CreateVector(Offer offer)
        {
            var vec = ComputeVector(offer.Name + " " + offer.Description);
            offer.VectorJson = JsonSerializer.Serialize(vec);
        }
    }
}
