using PASW.Application.Service.DTO;
using PASW.Application.Service.Interface;
using PASW.Domain.Entity;
using PASW.Domain.Entity.Enum;
using PASW.Domain.Interface.Repository;
using PASW.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PASW.Application.Service
{
    public class DiffService : IDiffService
    {
        private readonly IDiffRepository diffRepository;

        public DiffService(IDiffRepository diffRepository)
        {
            this.diffRepository = diffRepository;
        }

        public async Task PostDiffEntry(long id, Side side, string data)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var comparisonRequest = await diffRepository.GetById(id);

            if (comparisonRequest == null)
            {
                await InsertComparisonRequest(id, side, data);
                return;
            }

            UpdateComparisonRequest(side, data, comparisonRequest);
        }

        private void UpdateComparisonRequest(Side side, string data, ComparisonRequest comparisonRequest)
        {
            comparisonRequest.Left = side == Side.Left ? data : comparisonRequest.Left;
            comparisonRequest.Right = side == Side.Right ? data : comparisonRequest.Right;

            diffRepository.Update(comparisonRequest);
            diffRepository.SaveChanges();
        }

        public async Task<DiffResultDTO> Diff(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var comparisonRequest = await diffRepository.GetById(id);
            if (comparisonRequest == null)
            {
                throw new PASWException(ExceptionType.ComparisonRequestNotFound);
            }

            if (comparisonRequest.Left == null || comparisonRequest.Right == null)
            {
                throw new PASWException(ExceptionType.InsuficientDataForComparison);
            }

            return Diff(comparisonRequest);
        }

        private async Task InsertComparisonRequest(long id, Side side, string data)
        {
            var comparisonRequest = new ComparisonRequest
            {
                Id = id,
                Left = side == Side.Left ? data : null,
                Right = side == Side.Right ? data : null
            };

            await diffRepository.Insert(comparisonRequest);
            await diffRepository.SaveChangesAsync();
        }

        private DiffResultDTO Diff(ComparisonRequest comparisonRequest)
        {
            var result = new DiffResultDTO
            {
                Id = comparisonRequest.Id,
                Right = comparisonRequest.Right,
                Left = comparisonRequest.Left
            };

            if (comparisonRequest.Left.Equals(comparisonRequest.Right))
            {
                result.Equal = true;
            }

            result.SameSize = comparisonRequest.Right.Length == comparisonRequest.Left.Length;

            if (!result.SameSize)
            {
                return result;
            }

            result.DiffInsights = GetDiffInsights(result.Left, result.Right);

            return result;
        }

        private List<string> GetDiffInsights(string left, string right)
        {
            var result = new List<string>();
            var offset = 0;
            var diffLength = 0;
            var diffOffset = 0;

            while (offset < left.Length)
            {
                if (left[offset] != right[offset])
                {
                    if (diffOffset == 0) diffOffset = offset;
                    diffLength++;
                }
                else
                {
                    if (diffLength > 0)
                    {
                        result.Add($"Difference detected, starting at offset {diffOffset + 1} with length of {diffLength}.");
                    }
                        
                    diffLength = 0;
                    diffOffset = 0;
                }

                offset++;
            }

            if (diffLength > 0)
            {
                result.Add($"Difference detected, starting at offset {diffOffset + 1} with length of {diffLength}.");
            }
                
            return result;
        }
    }
}
