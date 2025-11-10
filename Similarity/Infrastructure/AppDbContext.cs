using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Offer> CatalogItems { get; set; }
}

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.CatalogItems.Any()) return;

        var items = new List<Offer>
        {
            // 📱 Smartfonlar
            new Offer{ Id=1, Type= OfferType.Product, Name="iPhone 13", Description="Apple smartphone 128GB", CategoryId=10, Price=699 },
            new Offer{ Id=2, Type= OfferType.Product, Name="iPhone 12", Description="Apple smartphone 64GB", CategoryId=10, Price=599 },
            new Offer{ Id=3, Type= OfferType.Product, Name="Samsung Galaxy S22", Description="Android smartphone 128GB", CategoryId=10, Price=749 },
            new Offer{ Id=4, Type= OfferType.Product, Name="Google Pixel 7", Description="Android smartphone 128GB", CategoryId=10, Price=679 },

            // 🧰 Xizmatlar (telefon)
            new Offer{ Id=5, Type= OfferType.Service, Name="iPhone Screen Repair", Description="Replace cracked iPhone screen for models X-13", CategoryId=11, Price=129 },
            new Offer{ Id=6, Type= OfferType.Service, Name="Phone Battery Replacement", Description="Battery replacement for smartphones", CategoryId=11, Price=49 },
            new Offer{ Id=7, Type= OfferType.Service, Name="Water Damage Repair", Description="Fix water-damaged phones", CategoryId=11, Price=99 },

            // 🎧 Aksessuarlar
            new Offer{ Id=8, Type= OfferType.Product, Name="AirPods Pro", Description="Wireless earbuds by Apple", CategoryId=12, Price=249 },
            new Offer{ Id=9, Type= OfferType.Product, Name="Samsung Galaxy Buds 2", Description="Wireless earbuds with ANC", CategoryId=12, Price=199 },
            new Offer{ Id=10, Type= OfferType.Product, Name="iPhone 13 Case", Description="Protective silicone case", CategoryId=12, Price=29 },

            // 💻 Noutbuklar
            new Offer{ Id=11, Type= OfferType.Product, Name="MacBook Air M2", Description="Apple laptop 8GB 256GB SSD", CategoryId=13, Price=1199 },
            new Offer{ Id=12, Type= OfferType.Product, Name="ASUS ZenBook 14", Description="Ultrabook with 16GB RAM", CategoryId=13, Price=999 },
            new Offer{ Id=13, Type= OfferType.Product, Name="Lenovo ThinkPad X1", Description="Business laptop with Intel i7", CategoryId=13, Price=1299 },

            // 🧑‍💻 Kompyuter xizmatlari
            new Offer{ Id=14, Type= OfferType.Service, Name="Laptop Screen Replacement", Description="Fix or replace damaged laptop screens", CategoryId=14, Price=150 },
            new Offer{ Id=15, Type= OfferType.Service, Name="Virus Removal Service", Description="Clean and secure your computer", CategoryId=14, Price=79 },
            new Offer{ Id=16, Type= OfferType.Service, Name="SSD Upgrade", Description="Upgrade your laptop or PC to SSD", CategoryId=14, Price=99 }};

        db.CatalogItems.AddRange(items);
        db.SaveChanges();
    }
}
