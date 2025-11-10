using Microsoft.EntityFrameworkCore;
using Microsoft.ML.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CatalogDb"));

builder.Services.AddSingleton<SimilarityService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
    var svc = scope.ServiceProvider.GetRequiredService<SimilarityService>();
    svc.BuildIndex(db.CatalogItems.ToList()); // build TF-IDF vectors once on startup
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();