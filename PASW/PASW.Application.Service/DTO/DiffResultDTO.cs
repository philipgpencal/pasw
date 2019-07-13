using System.Collections.Generic;

namespace PASW.Application.Service.DTO
{
    public class DiffResultDTO
    {
        public DiffResultDTO()
        {
            DiffInsights = new List<string>();
        }

        public long Id { get; set; }
        public bool Equal { get; set; }
        public bool SameSize { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public List<string> DiffInsights { get; set; }
    }
}
