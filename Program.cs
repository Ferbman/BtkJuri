using Microsoft.EntityFrameworkCore;
using BtkJuri.Data;
using BtkJuri.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ─── Yerel ağ üzerinden erişim için 0.0.0.0:5000 dinleme ───
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// ─── Servisler ───
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSignalR();

// ─── SQLite + EF Core ───
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=BtkJuri.db"));

var app = builder.Build();

// ─── Veritabanını otomatik oluştur ve seed et ───
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ─── Middleware pipeline ───
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<VotingHub>("/votingHub");

app.Run();
