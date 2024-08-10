namespace Common.Managers
{
    public interface IRelationalFrameworkManager : IFrameworkManager
    {
        TestResult SearchTwoRelatedTables(int samplesQuantity);
        TestResult SearchFourRelatedTables(int samplesQuantity);
        TestResult SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity);
        TestResult SearchWithSubQuery(int samplesQuantity);
        TestResult RemoveRelatedRecords(int samplesQuantity);
    }
}
