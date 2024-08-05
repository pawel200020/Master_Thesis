using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Facades
{
    public interface IRelationalFrameworkTestsFacade : IFrameworkTestsFacade
    {
        string SearchTwoRelatedTables(int samplesQuantity);
        string SearchFourRelatedTables(int samplesQuantity);
        string SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity);
        string SearchWithSubQuery(int samplesQuantity);
    }
}
