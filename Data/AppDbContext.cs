using Microsoft.EntityFrameworkCore;
using OnlineVotingSystem.Models;

namespace OnlineVotingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Poll> Polls { get; set; } = default!;
        public DbSet<PollOption> PollOptions { get; set; } = default!;
    }
}
