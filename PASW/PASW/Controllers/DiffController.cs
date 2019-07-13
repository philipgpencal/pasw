using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PASW.Application.Service.Interface;
using PASW.Domain.Entity.Enum;
using PASW.Filter;
using PASW.Models;

namespace PASW.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class DiffController : Controller
    {
        private readonly IDiffService diffService;

        public DiffController(IDiffService diffService)
        {
            this.diffService = diffService;
        }

        [Route("{id}/left")]
        [CustomExceptionFilter]
        [HttpPost]
        public async Task<ActionResult> PostLeftDiffEntry(long id, string data)
        {
            await diffService.PostDiffEntry(id, Side.Left, data);
            return Ok();
        }

        [Route("{id}/right")]
        [CustomExceptionFilter]
        [HttpPost]
        public async Task<ActionResult> PostRightDiffEntry(long id, string data)
        {
            await diffService.PostDiffEntry(id, Side.Right, data);
            return Ok();
        }
       
        [HttpGet("{id}")]
        [CustomExceptionFilter]
        public async Task<DiffResultModel> GetDiff(long id)
        {
            var diffResultDTO = await diffService.Diff(id);
            return DiffResultModel.BuildFromDTO(diffResultDTO);
        }
    }
}
