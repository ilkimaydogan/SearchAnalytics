// SearchAnalytics.DB/SearchAnalyticsDbContext.cs
using Microsoft.EntityFrameworkCore;
using SearchAnalytics.Core.Models;

namespace SearchAnalytics.DB;

public class SearchAnalyticsDbContext : DbContext
{
    public SearchAnalyticsDbContext(DbContextOptions<SearchAnalyticsDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<SearchEngine> SearchEngines { get; set; } = null!;
    public DbSet<Search> Searches { get; set; } = null!;
    public DbSet<SearchResult> SearchResults { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure SearchEngine
        modelBuilder.Entity<SearchEngine>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Pattern)
                .IsRequired();
                
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
            
        });
        
        // Configure Search
        modelBuilder.Entity<Search>(entity =>
        {
            entity.HasKey(s => s.Id);
            
            entity.Property(s => s.Keyword)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.SearchEngineId).HasColumnName("SearchEngineId");
                
            entity.Property(s => s.TargetUrl)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.HasOne(x=> x.SearchEngine)
                .WithMany(x=> x.Searches)
                .HasForeignKey(x=> x.SearchEngineId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure SearchResult
        modelBuilder.Entity<SearchResult>(entity =>
        {
            entity.HasKey(sr => sr.Id);
            
            entity.Property(sr => sr.Url)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.HasOne(x=> x.Search)
                .WithMany(x=> x.SearchResults)
                .HasForeignKey(x=> x.SearchId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Seed data for search engines
        modelBuilder.Entity<SearchEngine>().HasData(
            new SearchEngine 
            { 
                Id = 1, 
                Name = "Google",
                Pattern = @"<a\s+jsname=""UWckNb""\s+class=""zReHs""\s+href=""(?<hrefContent>[^""]+)""\s+[^>]*>.*?<h3\s+class=""LC20lb[^""]*""[^>]*>(.*?)</h3>",
                IsActive = true
            }
        );
    }
}