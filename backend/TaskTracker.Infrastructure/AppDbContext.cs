using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<CheckList> CheckLists { get; set; }
    public DbSet<CheckListItem> CheckListItems { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<CardAssignee> CardAssignees { get; set; }
}