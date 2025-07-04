using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Data;

public class ChatAppDBContext : DbContext
{
    public ChatAppDBContext(DbContextOptions<ChatAppDBContext> options) : base(options) { }

    // public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var stringListConverter = new ValueConverter<List<string>, string>(
            v => string.Join(',', v ?? new List<string>()),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
}
