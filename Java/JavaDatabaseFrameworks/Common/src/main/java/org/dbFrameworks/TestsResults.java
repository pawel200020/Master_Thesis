package org.dbFrameworks;

import java.util.ArrayList;
import java.util.List;

public class TestsResults {
    private List<TestResult> _testResults = new ArrayList<TestResult>();
    public void Add(TestResult testResult) { _testResults.add(testResult); }

    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder();
        sb.append("{" +
                "   \"Language\": Java"+
                "   \"Results\": [");
        for(TestResult testResult : _testResults){
            sb.append(testResult.toString());
        }
        sb.append("   ]'\n}");
        return sb.toString();
    }
}
