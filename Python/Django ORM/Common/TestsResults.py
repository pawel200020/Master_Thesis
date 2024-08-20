class TestResults:
    def __init__(self):
        self.tests = []
    def Add(self, test_result):
        self.tests.append (test_result)

    def __repr__(self):
        result ="""
"Language": "Python"
"Results": ["""
        for test in self.tests:
            result += str(test) + "\n"
        result+="     ]\n}"
        return result

