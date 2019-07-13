using PASW.Domain.Entity;
using PASW.Domain.Entity.Enum;

namespace PASW.Application.Service.Test.Factory
{
    public static class ComparisonRequestFactory
    {
        public static ComparisonRequest GetSidesNull(long id)
        {
            return new ComparisonRequest
            {
                Id = id
            };
        }

        public static ComparisonRequest GetSingleSideNull(long id, Side sideNullValue, string value)
        {
            return new ComparisonRequest
            {
                Id = id,
                Left = sideNullValue == Side.Left ? null : value,
                Right = sideNullValue == Side.Right ? null : value
            };
        }

        public static ComparisonRequest GetValid(long id, string rightValue, string leftValue)
        {
            return new ComparisonRequest
            {
                Id = id,
                Left = leftValue,
                Right = rightValue
            };
        }
    }
}
