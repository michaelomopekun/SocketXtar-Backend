using MongoDB.Driver;
using Domain.Entities;
using Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Data;

public class ChatAppDBContext : DbContext
{
    public ChatAppDBContext(DbContextOptions<ChatAppDBContext> options) : base(options) { }

    public DbSet<User> User { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new UserConfiguration());

        var stringListConverter = new ValueConverter<List<string>, string>(
            v => string.Join(',', v ?? new List<string>()),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
}
