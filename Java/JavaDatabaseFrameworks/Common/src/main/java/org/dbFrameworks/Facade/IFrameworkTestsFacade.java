package org.dbFrameworks.Facade;

public interface IFrameworkTestsFacade {
    String RunAllTests(int samplesQuantity, String fileName);
    String SingleRecordSearch(int samplesQuantity);
    String SetOfDataSearch(int samplesQuantity);
    String SetOfDataWithIsNullSearch(int samplesQuantity);
    String AddRecords(int samplesQuantity);
    String EditRecords(int samplesQuantity);
    String DeleteRecords(int samplesQuantity);
}
