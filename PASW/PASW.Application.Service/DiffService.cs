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
                throw new ComparisonNotFoundException();
            }

            if (comparisonRequest.Left == null || comparisonRequest.Right == null)
            {
                throw new DataComparisonNotEnoughException();
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

        /// <summary>
        /// Compare position and size from a comparison request.
        /// </summary>
        /// <param name="comparisonRequest">hashs object</param>
        /// <returns>Return objetct with compasion results</returns>
        private DiffResultDTO Diff(ComparisonRequest comparisonRequest)
        {
            var result = new DiffResultDTO
            {
                Id = comparisonRequest.Id,
                Right = comparisonRequest.Right,
                Left = comparisonRequest.Left
            };

            // Comparing positions
            if (comparisonRequest.Left.Equals(comparisonRequest.Right))
            {
                result.Equal = true;
            }

            // Comparing sizes
            result.SameSize = comparisonRequest.Right.Length == comparisonRequest.Left.Length;

            if (!result.SameSize)
            {
                return result;
            }

            // Collecting Differences between both hashs
            result.DiffInsights = GetDiffInsights(result.Left, result.Right);

            return result;
        }

        /// <summary>
        /// Get different insights comparing each char between two string hashs.
        /// </summary>
        /// <param name="left">left side hash</param>
        /// <param name="right">right side hash</param>
        /// <returns>Return a list of differences</returns>
        private List<string> GetDiffInsights(string left, string right)
        {
            var result = new List<string>();
            var offset = 0;
            var diffLength = 0;
            var diffOffset = 0;

            // Run over each character to find the diferences
            while (offset < left.Length)
            {
                if (left[offset] != right[offset])
                {
                    // If chars aren't equal, stay with the first difference position
                    if (diffOffset == 0)
                    {
                        diffOffset = offset;
                    }

                    // Include difference found on counter.
                    diffLength++;
                }
                else
                {
                    // Create an insight based on previous differences found
                    if (diffLength > 0)
                    {
                        result.Add($"Difference detected, starting at offset {diffOffset + 1} with length of {diffLength}.");
                    }

                    // Reset variables to move to the next search
                    diffLength = 0;
                    diffOffset = 0;
                }

                offset++;
            }

            // Check if the last char is different as well.
            if (diffLength > 0)
            {
                result.Add($"Difference detected, starting at offset {diffOffset + 1} with length of {diffLength}.");
            }
                
            return result;
        }
    }
}
