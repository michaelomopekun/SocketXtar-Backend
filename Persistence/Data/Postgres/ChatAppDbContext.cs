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
    public DbSet<FriendRequest> FriendRequests { get; set; } = null!;
    public DbSet<Friend> Friend { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new UserConfiguration());

        var stringListConverter = new ValueConverter<List<string>, string>(
            v => string.Join(',', v ?? new List<string>()),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());


        builder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(s => s.Sender)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(s => s.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(r => r.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Friend>(builder =>
        {
            builder.HasKey(f => f.Id);

            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.FriendUser)
                .WithMany()
                .HasForeignKey(f => f.FriendUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(f => new { f.UserId, f.FriendUserId })
                .IsUnique();

            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

    }
}
