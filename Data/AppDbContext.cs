using Microsoft.EntityFrameworkCore;
using BtkJuri.Models;

namespace BtkJuri.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Vote -> Project relationship
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Project)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed initial projects
        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Name = "Akıllı Sera Sistemi", Developer = "Takım Alpha", Description = "IoT sensörleri ile sera koşullarını otomatik izleyen ve yöneten akıllı tarım çözümü." },
            new Project { Id = 2, Name = "Sağlık Asistanı", Developer = "Takım Beta", Description = "Yapay zeka destekli semptom analizi ve sağlık önerileri sunan mobil uygulama." },
            new Project { Id = 3, Name = "EcoTrack", Developer = "Takım Gamma", Description = "Karbon ayak izini takip eden ve sürdürülebilir yaşam önerileri sunan platform." },
            new Project { Id = 4, Name = "EduConnect", Developer = "Takım Delta", Description = "Öğrenci ve mentorları eşleştiren, canlı oturum ve kaynak paylaşımı sağlayan eğitim platformu." },
            new Project { Id = 5, Name = "SmartPark", Developer = "Takım Epsilon", Description = "Şehir içi otopark alanlarını gerçek zamanlı izleyen ve yönlendiren akıllı park sistemi." }
        );
    }
}
