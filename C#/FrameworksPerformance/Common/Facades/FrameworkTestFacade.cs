using Common.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Managers;

namespace Common.Facades
{
    public class FrameworkTestFacade(IFrameworkManager manager, JsonManager jsonManager) : IFrameworkTestsFacade
    {
        private readonly IFrameworkManager _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        private readonly JsonManager _jsonManager = jsonManager ?? throw new ArgumentNullException(nameof(jsonManager));

        public string RunAllTests(int samplesQuantity, string fileName)
        {
            var testsResults = new TestsResults();
            testsResults.Add(_manager.SingleRecordSearch(samplesQuantity));
            testsResults.Add(_manager.SetOfDataSearch(samplesQuantity));
            testsResults.Add(_manager.SetOfDataWithIsNullSearch(samplesQuantity));
            testsResults.Add(_manager.AddRecords(samplesQuantity));
            testsResults.Add(_manager.EditRecords(samplesQuantity));
            testsResults.Add(_manager.DeleteRecords(samplesQuantity));

            _jsonManager.SaveTestResultAsFile(fileName, testsResults);
            return _jsonManager.ConvertToJson(testsResults);
        }

        public string SingleRecordSearch(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SingleRecordSearch(samplesQuantity));

        public string SetOfDataSearch(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SetOfDataSearch(samplesQuantity));

        public string SetOfDataWithIsNullSearch(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.SetOfDataWithIsNullSearch(samplesQuantity));

        public string AddRecords(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.AddRecords(samplesQuantity));

        public string EditRecords(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.EditRecords(samplesQuantity));

        public string DeleteRecords(int samplesQuantity) => _jsonManager.ConvertToJson(_manager.DeleteRecords(samplesQuantity));
    }
}
