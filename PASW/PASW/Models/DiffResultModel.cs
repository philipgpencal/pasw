using PASW.Application.Service.DTO;
using System.Collections.Generic;

namespace PASW.Models
{
    public class DiffResultModel
    {
        public DiffResultModel()
        {
            DiffInsights = new List<string>();
        }

        public long Id { get; set; }
        public bool Equal { get; set; }
        public bool SameSize { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public List<string> DiffInsights { get; set; }

        public static DiffResultModel BuildFromDTO(DiffResultDTO diffResultDTO)
        {
            return new DiffResultModel
            {
                Id = diffResultDTO.Id,
                Equal = diffResultDTO.Equal,
                SameSize = diffResultDTO.SameSize,
                DiffInsights = diffResultDTO.DiffInsights,
                Left = diffResultDTO.Left,
                Right = diffResultDTO.Right
            };
        }
    }
}
