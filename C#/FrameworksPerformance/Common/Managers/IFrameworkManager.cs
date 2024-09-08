namespace Common.Managers
{
    public interface IFrameworkManager
    {
        TestResult SingleRecordSearch(int samplesQuantity);
        TestResult SetOfDataSearch(int samplesQuantity);
        TestResult SetOfDataWithIsNullSearch(int samplesQuantity);
        TestResult AddRecords(int samplesQuantity);
        TestResult EditRecords(int samplesQuantity);
        TestResult DeleteRecords(int samplesQuantity);
        TestResult SearchTwoRelatedTables(int samplesQuantity);
    }
}
