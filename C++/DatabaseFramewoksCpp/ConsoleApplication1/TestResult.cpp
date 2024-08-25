#include <algorithm>
#include <sstream>
#include <string>

class TestResult
{
private:
	int* _samples;
	int _currentPosition = 0;
	std::string _activityName;



	double sum ()
	{
		double sum = 0;
		for (int i = 0; i < _currentPosition; i++) {
			sum = sum + _samples[i];
		}
		return sum;
	}

	double median() {
		std::sort(_samples, _samples + _currentPosition);
		if (_currentPosition % 2 != 0)
			return _samples[_currentPosition / 2];
		return (_samples[(_currentPosition - 1) / 2] + _samples[_currentPosition / 2]) / 2.0;
	}

	double averange()
	{
		return sum() / (_currentPosition);
	}

	double max()
	{
		double max = 0;
		for(int i = 0; i < _currentPosition; i++)
		{
			if (_samples[i] > max)
				max = _samples[i];
		}
		return max;
	}

	double min()
	{
		double min = INT_MAX;
		for (int i = 0; i < _currentPosition; i++)
		{
			if (_samples[i] < min)
				min = _samples[i];
		}
		return min;
	}

	double variance()
	{
		double variance = 0;
		double t = _samples[0];
		for (int i = 1; i < _currentPosition; i++)
		{
			t += _samples[i];
			double diff = ((i + 1) * _samples[i]) - t;
			variance += (diff * diff) / ((i + 1.0) * i);
		}
		return variance / (_currentPosition - 1);
	}

	double StandardDeviation()
	{
		return sqrt(variance());
	}

public:
	TestResult(int samplesQuantity, std::string activityName)
	{
		_samples = new int[samplesQuantity];
		_activityName = activityName;
	}

	void AddMeasure(int value)
	{
		_samples[_currentPosition] = value;
		_currentPosition++;
	}

	std::string GetTestResultAsString()
	{
		std::stringstream ss;
		ss << "{\n	\"Activity\": \"" << _activityName << "\"\n";
		ss << "	    \"AverageTimeElapsed\":" << averange() << "\n";
		ss << "	    \"MedianTimeElapsed\":" << median() << "\n";
		ss << "	    \"StandardDeviationTimeElapsed\":" << StandardDeviation() << "\n";
		ss << "  	\"MaxTimeElapsed\":" << max() << "\n";
		ss << "	    \"MinTimeElapsed\"" << min() << "\n";
		ss << "	    \"VarianceTimeElapsed\":" << variance() << "\n";
		ss << "	    \"TotalTimeElapsed\"" << sum() << "\n}";
		return ss.str();
	}
	~TestResult()
	{
		delete _samples;
	}
};
