namespace PASW.Util.Exceptions
{
    public class ComparisonNotFoundException: PASWException
    {
        public ComparisonNotFoundException() : base("Comparison request was not found.")
        {

        }
    }
}
