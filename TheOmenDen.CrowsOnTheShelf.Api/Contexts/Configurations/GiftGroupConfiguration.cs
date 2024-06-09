using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheOmenDen.CrowsOnTheShelf.Api.Models;

namespace TheOmenDen.CrowsOnTheShelf.Api.Contexts.Configurations;

internal sealed class GiftGroupConfiguration : IEntityTypeConfiguration<GiftGroup>
{
    public void Configure(EntityTypeBuilder<GiftGroup> entity)
    {
        entity.ToTable("GiftGroups", "CAH");
        entity.HasKey(e => e.Id).IsClustered();
        entity.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();
        entity.Property(e => e.Description)
            .HasMaxLength(2000)
            .IsRequired();
        entity.Property(e => e.CreatedAt)
            .IsRequired();
        entity.Property(e => e.EventTakesPlaceAt)
            .IsRequired();
        entity.HasOne(e => e.InviteCode)
            .WithOne(l => l.Group)
            .HasForeignKey<InviteCode>(e => e.GroupId);
        entity.HasMany(e => e.Members)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GiftGroupId);
    }
}