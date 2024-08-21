from colorama import Fore
from colorama import Style

from Common.TestsResults import TestResults


class FrameworkMenu:
    def __init__(self, frameworkName, frameworkTests):
        self.frameworkTests = frameworkTests
        self.Operations = """ Selected framework - {frameworkName}
    Choose operation:

    A - Run All Tests and save to file

    Single table tests:
    B - Single record search
    C - Set of records search
    D - Set of records search with isNull command
    E - Adding single records
    F - Edit single record
    G - Delete single record""".format(frameworkName=frameworkName)

    def print_result(self, result):
        print(f"{Fore.GREEN}")
        print(result)
        print(f"{Style.RESET_ALL}")
        input("Press Enter to continue...")

    def showMenu(self):
        while True:
            print(self.Operations)
            key = input()
            print("number of repetitions:")
            rep = input()
            match key:
                 case "A":
                    testsResults = TestResults()
                    testsResults.Add(self.frameworkTests.singleRecordSearch(int(rep)))
                    testsResults.Add(self.frameworkTests.setOfDataSearch (int(rep)))
                    testsResults.Add(self.frameworkTests.setOfDataWithIsNullSearch(int(rep)))
                    testsResults.Add(self.frameworkTests.addRecords(int(rep)))
                    testsResults.Add(self.frameworkTests.editRecords(int(rep)))
                    testsResults.Add(self.frameworkTests.deleteRecords(int(rep)))
                    self.print_result(testsResults)
                 case "B":
                     result = self.frameworkTests.singleRecordSearch(int(rep))
                     self.print_result(result)
                 case "C":
                     result = self.frameworkTests.setOfDataSearch (int(rep))
                     self.print_result(result)
                 case "D":
                     result = self.frameworkTests.setOfDataWithIsNullSearch(int(rep))
                     self.print_result(result)
                 case "E":
                     result = self.frameworkTests.addRecords(int(rep))
                     self.print_result(result)
                 case "F":
                     result = self.frameworkTests.editRecords(int(rep))
                     self.print_result(result)
                 case "G":
                     result = self.frameworkTests.deleteRecords(int(rep))
                     self.print_result(result)
                 case "X":
                     return