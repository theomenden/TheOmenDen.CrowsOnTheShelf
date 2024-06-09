using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsOnTheShelf.Api.Models;

namespace TheOmenDen.CrowsOnTheShelf.Api.Contexts;

internal sealed class CrowsOnTheShelfContext(DbContextOptions<CrowsOnTheShelfContext> options) : DbContext(options)
{
    public DbSet<InviteCode> InviteCodes { get; set; }
    public DbSet<GiftGroup> Groups { get; set; }
    public DbSet<GiftGroupMember> Members { get; set; }


}