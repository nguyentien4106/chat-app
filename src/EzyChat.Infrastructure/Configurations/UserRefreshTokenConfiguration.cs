using EzyChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzyChat.Infrastructure.Configurations;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasOne(x => x.ApplicationUser)
            .WithOne(x => x.RefreshToken)
            .HasForeignKey<UserRefreshToken>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}