from colorama import Fore
from colorama import Style

from Common.TestsResults import TestResults


class RelationalFrameworkMenu:
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
    G - Delete single record

    Multiple table tests:
    H - Searching in two connected tables
    I - Searching in four connected tables
    J - Searching for records which does not have connection
    K - Searching with subquery
    L - Remove related records
    X - Main menu""".format(frameworkName=frameworkName)

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
                    testsResults.Add(self.frameworkTests.searchTwoRelatedTables(int(rep)))
                    testsResults.Add(self.frameworkTests.serchFourRelatedTables(int(rep)))
                    testsResults.Add(self.frameworkTests.searchRecordsWhichDoesNotHaveConnection(int(rep)))
                    testsResults.Add(self.frameworkTests.searchWithSubQuery(int(rep)))
                    testsResults.Add(self.frameworkTests.removeRelatedRecords(int(rep)))
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
                 case "H":
                     result = self.frameworkTests.searchTwoRelatedTables(int(rep))
                     self.print_result(result)
                 case "I":
                     result = self.frameworkTests.serchFourRelatedTables(int(rep))
                     self.print_result(result)
                 case "J":
                     result = self.frameworkTests.searchRecordsWhichDoesNotHaveConnection(int(rep))
                     self.print_result(result)
                 case "K":
                    result = self.frameworkTests.searchWithSubQuery(int(rep))
                    self.print_result(result)
                 case "L":
                    result = self.frameworkTests.removeRelatedRecords(int(rep))
                    self.print_result(result)
                 case "X":
                     return