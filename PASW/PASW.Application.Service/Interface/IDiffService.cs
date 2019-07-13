using System.Threading.Tasks;
using PASW.Application.Service.DTO;
using PASW.Domain.Entity.Enum;

namespace PASW.Application.Service.Interface
{
    public interface IDiffService
    {
        Task PostDiffEntry(long id, Side side, string data);
        Task<DiffResultDTO> Diff(long id);
    }
}
