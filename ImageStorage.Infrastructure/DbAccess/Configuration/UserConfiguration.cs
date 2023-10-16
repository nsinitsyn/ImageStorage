using ImageStorage.Domain.Entities;
using ImageStorage.Infrastructure.DbAccess.Links;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageStorage.Infrastructure.DbAccess.Configuration;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).HasMaxLength(256);
        builder.Property(x => x.Email).HasMaxLength(256);

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasMany(x => x.Friends)
            .WithMany(x => x.UsersAddedAsFriend)
            .UsingEntity<UserFriendLink>(
                b => b.HasOne(x => x.Friend).WithMany().HasForeignKey(x => x.FriendId),
                b => b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId))
            .ToTable($"{nameof(UserFriendLink)}s");
    }
}
