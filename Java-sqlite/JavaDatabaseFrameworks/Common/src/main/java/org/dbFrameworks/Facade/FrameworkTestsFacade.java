package org.dbFrameworks.Facade;

import org.dbFrameworks.Managers.IFrameworkManager;
import org.dbFrameworks.TestsResults;

public class FrameworkTestsFacade implements IFrameworkTestsFacade {
    private final IFrameworkManager _manager;

    public FrameworkTestsFacade(IFrameworkManager manager) {
        _manager = manager;
    }
    @Override
    public String RunAllTests(int samplesQuantity, String fileName) {
        var testsResults = new TestsResults();
        testsResults.Add(_manager.SingleRecordSearch(samplesQuantity));
        testsResults.Add(_manager.SetOfDataSearch(samplesQuantity));
        testsResults.Add(_manager.SetOfDataWithIsNullSearch(samplesQuantity));
        testsResults.Add(_manager.AddRecords(samplesQuantity));
        testsResults.Add(_manager.EditRecords(samplesQuantity));
        testsResults.Add(_manager.DeleteRecords(samplesQuantity));
        return testsResults.toString();
    }

    @Override
    public String SingleRecordSearch(int samplesQuantity) {
        return _manager.SingleRecordSearch(samplesQuantity).toString();
    }

    @Override
    public String SetOfDataSearch(int samplesQuantity) {
        return _manager.SetOfDataSearch(samplesQuantity).toString();
    }

    @Override
    public String SetOfDataWithIsNullSearch(int samplesQuantity) {
        return _manager.SetOfDataWithIsNullSearch(samplesQuantity).toString();
    }

    @Override
    public String AddRecords(int samplesQuantity) {
        return _manager.AddRecords(samplesQuantity).toString();
    }

    @Override
    public String EditRecords(int samplesQuantity) {
        return _manager.EditRecords(samplesQuantity).toString();
    }

    @Override
    public String DeleteRecords(int samplesQuantity) {
        return _manager.DeleteRecords(samplesQuantity).toString();
    }
}
