package org.dbFrameworks;

import java.util.Arrays;

public class TestResult {

    private double _avgTimeElapsed;
    private double _medianTimeElapsed;
    private double _standardDeviationTimeElapsed;
    private double _maxTimeElapsed;
    private double _minTimeElapsed;
    private double _varianceTimeElapsed;
    private double _totalTimeElapsed;

    private int _samples;
    private int _addedSamples;
    private double[] _attemptsList;
    private boolean _resultsPrepared;
    private String _actvivity;

    public TestResult(int samples, String activity) {
        _attemptsList = new double[samples];
        _samples = samples;
        _actvivity = activity;
    }

    public boolean AddMeasure(double time)
    {
        if (_addedSamples >= _samples)
            return false;

        _attemptsList[_addedSamples]=time;
        _addedSamples++;
        return true;
    }

    private void PrepareResults(){
        _avgTimeElapsed = Arrays.stream(_attemptsList).average().getAsDouble();
        _medianTimeElapsed = getMedian(_attemptsList);
        _standardDeviationTimeElapsed =calculateStandardDeviation(_attemptsList);
        _maxTimeElapsed = Arrays.stream(_attemptsList).max().getAsDouble();
        _minTimeElapsed = Arrays.stream(_attemptsList).min().getAsDouble();
        _varianceTimeElapsed = calculateVariance(_attemptsList);
        _totalTimeElapsed = Arrays.stream(_attemptsList).sum();
        _resultsPrepared = true;
    }
    private double getMedian(double[] array){
        Arrays.sort(array);
        return array.length % 2 == 0
                ? ((double)array[array.length/2] + (double)array[array.length/2 - 1])/2
                : (double) array[array.length/2];

    }

    private double calculateVariance(double[] array){
        double sum = 0.0;
        for (double i : array) {
            sum += i;
        }

        int length = array.length;
        double mean = sum / length;

        double variance  = 0.0;
        for (double num : array) {
            variance += Math.pow(num - mean, 2);
        }

        return variance / length;
    }

    private double calculateStandardDeviation(double[] array) {
        return Math.sqrt(calculateVariance(array));
    }

    public double get_avgTimeElapsed() {
        return _avgTimeElapsed;
    }

    public double get_medianTimeElapsed() {
        return _medianTimeElapsed;
    }

    public double get_standardDeviationTimeElapsed() {
        return _standardDeviationTimeElapsed;
    }

    public double get_maxTimeElapsed() {
        return _maxTimeElapsed;
    }

    public double get_minTimeElapsed() {
        return _minTimeElapsed;
    }

    public double get_varianceTimeElapsed() {
        return _varianceTimeElapsed;
    }

    public double get_totalTimeElapsed() {
        return _totalTimeElapsed;
    }

    @Override
    public String toString() {
        if(!_resultsPrepared)
            PrepareResults();
        return '{' +
                "   \"Activity\":" + _actvivity +",\n"+
                "   \"AverageTimeElapsed\":" + _avgTimeElapsed +",\n"+
                "   \"MedianTimeElapsed\":" + _medianTimeElapsed +",\n"+
                "   \"StandardDeviationTimeElapsed\":" + _standardDeviationTimeElapsed +",\n"+
                "   \"MaxTimeElapsed\":" + _maxTimeElapsed +",\n"+
                "   \"MinTimeElapsed\":" + _minTimeElapsed +",\n"+
                "   \"VarianceTimeElapsed\":" + _varianceTimeElapsed +",\n"+
                "   \"TotalTimeElapsed\":" + _totalTimeElapsed +",\n"+
                '}';
    }
}
