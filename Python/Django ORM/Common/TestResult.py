import numpy as np
class TestResult:
    def __init__(self, repetitions, activity):
        self.repetitions = repetitions
        self.activity = activity
        self.samples = np.empty(repetitions,dtype=float)
        self.curr = 0

    def add_sample(self, sample):
        self.samples[self.curr] = sample
        self.curr += 1

    def __repr__(self):
        return ("{"+"""
    "Activity": {},
    "AverageTimeElapsed": {},
    "MedianTimeElapsed": {},
    "StandardDeviationTimeElapsed": {},
    "MaxTimeElapsed": {},
    "MinTimeElapsed": {},
    "VarianceTimeElapsed": {},
    "TotalTimeElapsed": {},
""".format(self.activity,
                    np.average(self.samples),
                    np.median(self.samples),
                    np.std(self.samples),
                    np.max(self.samples),
                    np.min(self.samples),
                    np.var(self.samples),
                    np.sum(self.samples))+"}")
