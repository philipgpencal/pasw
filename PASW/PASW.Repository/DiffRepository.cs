using PASW.Domain.Entity;
using PASW.Domain.Interface.Repository;
using PASW.Repository.DBContext;

namespace PASW.Repository
{
    public class DiffRepository : BaseRepository<ComparisonRequest>, IDiffRepository
    {
        public DiffRepository(PASWContext context) : base(context) { }
    }
}
