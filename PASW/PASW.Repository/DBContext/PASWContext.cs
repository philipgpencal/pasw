using Microsoft.EntityFrameworkCore;
using PASW.Domain.Entity;

namespace PASW.Repository.DBContext
{
    public class PASWContext : DbContext
    {
        public PASWContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ComparisonRequest> ComparisonRequests { get; set; }
    }
}
