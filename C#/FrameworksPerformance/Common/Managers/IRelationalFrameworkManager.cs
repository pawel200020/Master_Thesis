namespace Common.Managers
{
    public interface IRelationalFrameworkManager : IFrameworkManager
    {
        TestResult SearchTwoRelatedTables(int samplesQuantity);
        TestResult SearchThreeRelatedTables(int samplesQuantity);
        TestResult SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity);
        TestResult SearchWithSubQuery(int samplesQuantity);
    }
}
