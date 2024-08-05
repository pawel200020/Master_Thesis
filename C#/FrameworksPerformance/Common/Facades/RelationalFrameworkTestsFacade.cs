using Common.Managers;
using Common.Utilites;

namespace Common.Facades
{
    public class RelationalFrameworkTestsFacade : IRelationalFrameworkTestsFacade
    {
        private readonly IRelationalFrameworkManager _manager;
        private readonly JsonManager _jsonManager;

        public RelationalFrameworkTestsFacade(IRelationalFrameworkManager manager, JsonManager jsonManager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _jsonManager = jsonManager ?? throw new ArgumentNullException(nameof(jsonManager));
        }

        public string RunAllTests(int samplesQuantity, string fileName)
        {
            var testsResults = new TestsResults();
            testsResults.Add(_manager.SingleRecordSearch(samplesQuantity));
            testsResults.Add(_manager.SetOfDataSearch(samplesQuantity));
            testsResults.Add(_manager.SetOfDataWithIsNullSearch(samplesQuantity));
            testsResults.Add(_manager.AddRecords(samplesQuantity));
            testsResults.Add(_manager.EditRecords(samplesQuantity));
            testsResults.Add(_manager.DeleteRecords(samplesQuantity));
            testsResults.Add(_manager.SearchTwoRelatedTables(samplesQuantity));
            testsResults.Add(_manager.SearchThreeRelatedTables(samplesQuantity));
            testsResults.Add(_manager.SearchRecordsWhichDoesNotHaveConnection(samplesQuantity));
            testsResults.Add(_manager.SearchWithSubQuery(samplesQuantity));

            _jsonManager.SaveTestResultAsFile(fileName,testsResults);
            return _jsonManager.ConvertToJson(testsResults);
        }

        public string SingleRecordSearch(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SingleRecordSearch(samplesQuantity));

        public string SetOfDataSearch(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SetOfDataSearch(samplesQuantity));

        public string SetOfDataWithIsNullSearch(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SetOfDataWithIsNullSearch(samplesQuantity));

        public string AddRecords(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.AddRecords(samplesQuantity));

        public string EditRecords(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.EditRecords(samplesQuantity));

        public string DeleteRecords(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.DeleteRecords(samplesQuantity));

        public string SearchTwoRelatedTables(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SearchTwoRelatedTables(samplesQuantity));

        public string SearchFourRelatedTables(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SearchThreeRelatedTables(samplesQuantity));

        public string SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SearchRecordsWhichDoesNotHaveConnection(samplesQuantity));

        public string SearchWithSubQuery(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SearchWithSubQuery(samplesQuantity));
    }
}
