using MathNet.Numerics.Statistics;

namespace Common
{
    public class TestResult
    {
        public string Activity { get; }

        public double AverageTimeElapsed
        {
            get
            {
                if (_avgTimeElapsedCached is not null)
                    return _avgTimeElapsedCached.Value;

                if (_addedSamples + 1 == _samples)
                {
                    _avgTimeElapsedCached = _attemptsList.Average();
                    return _avgTimeElapsedCached.Value;
                }
                return -1;
            }
        }

        public double MedianTimeElapsed
        {
            get
            {
                if (_medianTimeElapsedCached is not null)
                    return _medianTimeElapsedCached.Value;

                if (_addedSamples + 1 == _samples)
                {
                    _medianTimeElapsedCached = _attemptsList.Median();
                    return _medianTimeElapsedCached.Value;
                }
                return -1;
            }
        }

        public double StandardDeviationTimeElapsed
        {
            get
            {
                if (_standardDeviationTimeElapsedCached is not null)
                    return _standardDeviationTimeElapsedCached.Value;

                if (_addedSamples + 1 == _samples)
                {
                    _standardDeviationTimeElapsedCached = _attemptsList.StandardDeviation();
                    return _standardDeviationTimeElapsedCached.Value;
                }
                return -1;
            }
        }

        public double MaxTimeElapsed
        {
            get
            {
                if (_maxTimeElapsedCached is not null)
                    return _maxTimeElapsedCached.Value;

                if (_addedSamples + 1 == _samples)
                {
                    _maxTimeElapsedCached = _attemptsList.Maximum();
                    return _maxTimeElapsedCached.Value;
                }
                return -1;
            }
        }

        public double MinTimeElapsed
        {
            get
            {
                if (_minTimeElapsedCached is not null)
                    return _minTimeElapsedCached.Value;

                if (_addedSamples + 1 == _samples)
                {
                    _minTimeElapsedCached = _attemptsList.Minimum();
                    return _minTimeElapsedCached.Value;
                }
                return -1;
            }
        }

        public double VarianceTimeElapsed
        {
            get
            {
                if (_varianceTimeElapsedCached is not null)
                    return _varianceTimeElapsedCached.Value;

                if (_addedSamples + 1 == _samples)
                {
                    _varianceTimeElapsedCached = _attemptsList.Variance();
                    return _varianceTimeElapsedCached.Value;
                }
                return -1;
            }
        }


        private double? _avgTimeElapsedCached;
        private double? _medianTimeElapsedCached;
        private double? _standardDeviationTimeElapsedCached;
        private double? _maxTimeElapsedCached;
        private double? _minTimeElapsedCached;
        private double? _varianceTimeElapsedCached;


        private readonly int _samples;
        private int _addedSamples;
        private readonly double[] _attemptsList;

        public TestResult(int samples, string activity)
        {
            _attemptsList = new double[samples];
            _samples = samples;
            Activity = activity;
        }

        public bool AddMeasure(double time)
        {
            if (_addedSamples >= _samples) 
                return false;

            _attemptsList[_addedSamples]=time;
            _addedSamples++;
            return true;
        }
        
    }
}
