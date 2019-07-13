using System.ComponentModel;

namespace PASW.Util.Exceptions
{
    public enum ExceptionType
    {
        None = 0,
        [Description("Comparison request was not found.")]
        ComparisonRequestNotFound = 1,
        [Description("Data for comparison is not enough.")]
        InsuficientDataForComparison = 2
    }
}
