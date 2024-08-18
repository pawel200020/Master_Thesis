package org.dbFrameworks.Facade;

public interface IRelationalFrameworkTestsFacade extends IFrameworkTestsFacade {
    String SearchTwoRelatedTables(int samplesQuantity);
    String SearchFourRelatedTables(int samplesQuantity);
    String SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity);
    String SearchWithSubQuery(int samplesQuantity);
    String RemoveRelatedRecords(int samplesQuantity);
}
