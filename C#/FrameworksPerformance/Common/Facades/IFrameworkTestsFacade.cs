using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Facades
{
    public interface IFrameworkTestsFacade
    {
        string RunAllTests(int samplesQuantity, string fileName);
        string SingleRecordSearch(int samplesQuantity);
        string SetOfDataSearch(int samplesQuantity);
        string SetOfDataWithIsNullSearch(int samplesQuantity);
        string AddRecords(int samplesQuantity);
        string EditRecords(int samplesQuantity);
        string DeleteRecords(int samplesQuantity);
        string SearchTwoRelatedTables(int samplesQuantity);
    }
}
