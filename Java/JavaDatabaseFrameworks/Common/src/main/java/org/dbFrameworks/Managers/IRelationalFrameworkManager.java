package org.dbFrameworks.Managers;

import org.dbFrameworks.TestResult;

public interface IRelationalFrameworkManager extends IFrameworkManager{
    TestResult SearchTwoRelatedTables(int samplesQuantity);
    TestResult SearchFourRelatedTables(int samplesQuantity);
    TestResult SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity);
    TestResult SearchWithSubQuery(int samplesQuantity);
    TestResult RemoveRelatedRecords(int samplesQuantity);
}
