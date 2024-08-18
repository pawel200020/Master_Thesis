package org.dbFrameworks.Facade;

import org.dbFrameworks.Managers.IRelationalFrameworkManager;
import org.dbFrameworks.TestsResults;

public class RelationalFrameworkTestsFacade implements IRelationalFrameworkTestsFacade {
    IRelationalFrameworkManager _relationalFrameworkManager;

    public RelationalFrameworkTestsFacade(IRelationalFrameworkManager relationalFrameworkManager) {
        _relationalFrameworkManager = relationalFrameworkManager;
    }

    @Override
    public String RunAllTests(int samplesQuantity, String fileName) {
        var testsResults = new TestsResults();
        testsResults.Add(_relationalFrameworkManager.SingleRecordSearch(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.SetOfDataSearch(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.SetOfDataWithIsNullSearch(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.AddRecords(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.EditRecords(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.DeleteRecords(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.SearchTwoRelatedTables(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.SearchFourRelatedTables(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.SearchRecordsWhichDoesNotHaveConnection(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.SearchWithSubQuery(samplesQuantity));
        testsResults.Add(_relationalFrameworkManager.RemoveRelatedRecords(samplesQuantity));
        return testsResults.toString();
    }

    @Override
    public String SingleRecordSearch(int samplesQuantity) {
        return _relationalFrameworkManager.SingleRecordSearch(samplesQuantity).toString();
    }

    @Override
    public String SetOfDataSearch(int samplesQuantity) {
        return _relationalFrameworkManager.SetOfDataSearch(samplesQuantity).toString();
    }

    @Override
    public String SetOfDataWithIsNullSearch(int samplesQuantity) {
        return _relationalFrameworkManager.SetOfDataWithIsNullSearch(samplesQuantity).toString();
    }

    @Override
    public String AddRecords(int samplesQuantity) {
        return _relationalFrameworkManager.AddRecords(samplesQuantity).toString();
    }

    @Override
    public String EditRecords(int samplesQuantity) {
        return _relationalFrameworkManager.EditRecords(samplesQuantity).toString();
    }

    @Override
    public String DeleteRecords(int samplesQuantity) {
        return _relationalFrameworkManager.DeleteRecords(samplesQuantity).toString();
    }

    @Override
    public String SearchTwoRelatedTables(int samplesQuantity) {
        return _relationalFrameworkManager.SearchTwoRelatedTables(samplesQuantity).toString();
    }

    @Override
    public String SearchFourRelatedTables(int samplesQuantity) {
        return _relationalFrameworkManager.SearchFourRelatedTables(samplesQuantity).toString();
    }

    @Override
    public String SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity) {
        return _relationalFrameworkManager.SearchRecordsWhichDoesNotHaveConnection(samplesQuantity).toString();
    }

    @Override
    public String SearchWithSubQuery(int samplesQuantity) {
        return _relationalFrameworkManager.SearchWithSubQuery(samplesQuantity).toString();
    }

    @Override
    public String RemoveRelatedRecords(int samplesQuantity) {
        return _relationalFrameworkManager.RemoveRelatedRecords(samplesQuantity).toString();
    }

}
