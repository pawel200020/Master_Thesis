package org.dbFrameworks.Menu;

import org.dbFrameworks.Facade.IRelationalFrameworkTestsFacade;

import java.util.Scanner;

public class RelationalFrameworkMenu {
    private final IRelationalFrameworkTestsFacade _relationalFrameworkTestsFacade;
    private String _frameworkName;
    public static final String TEXT_GREEN  = "\u001B[32m";
    public static final String TEXT_RESET  = "\u001B[0m";

    protected String Operations(String name) {return  String.format("""
    Selected framework - %s
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
    X - Main menu
""",name);}

    public RelationalFrameworkMenu(IRelationalFrameworkTestsFacade relationalFrameworkTestsFacade, String frameworkName) {
        _relationalFrameworkTestsFacade = relationalFrameworkTestsFacade;
        _frameworkName = frameworkName;
    }

    private void DisplayResult(String result){
        System.out.println(TEXT_GREEN + "Result\n----------------------\n"+result+"\n----------------------");
        System.out.println(TEXT_RESET+"Press any key to continue...");
        var scanner = new Scanner(System.in);
        scanner.nextLine();
    }

    public void Display(){
        while (true){
            System.out.println(Operations(_frameworkName));
            var scanner = new Scanner(System.in);
            var input = scanner.next();
            System.out.println("\n Number of repetitions\n");
            var repeititions = scanner.nextInt();
            switch (input){
                case "A":
                    DisplayResult(_relationalFrameworkTestsFacade.RunAllTests(repeititions,"test.txt"));
                    break;
                case "B":
                    DisplayResult(_relationalFrameworkTestsFacade.SingleRecordSearch(repeititions));
                    break;
                case "C":
                    DisplayResult(_relationalFrameworkTestsFacade.SetOfDataSearch(repeititions));
                    break;
                case "D":
                    DisplayResult(_relationalFrameworkTestsFacade.SetOfDataWithIsNullSearch(repeititions));
                    break;
                case "E":
                    DisplayResult(_relationalFrameworkTestsFacade.AddRecords(repeititions));
                    break;
                case "F":
                    DisplayResult(_relationalFrameworkTestsFacade.EditRecords(repeititions));
                    break;
                case "G":
                    DisplayResult(_relationalFrameworkTestsFacade.DeleteRecords(repeititions));
                    break;
                case "H":
                    DisplayResult(_relationalFrameworkTestsFacade.SearchTwoRelatedTables(repeititions));
                    break;
                case "I":
                    DisplayResult(_relationalFrameworkTestsFacade.SearchFourRelatedTables(repeititions));
                    break;
                case "J":
                    DisplayResult(_relationalFrameworkTestsFacade.SearchRecordsWhichDoesNotHaveConnection(repeititions));
                    break;
                case "K":
                    DisplayResult(_relationalFrameworkTestsFacade.SearchWithSubQuery(repeititions));
                    break;
                case "L":
                    DisplayResult(_relationalFrameworkTestsFacade.RemoveRelatedRecords(repeititions));
                    break;
                case "X":
                    return;
            }
        }
    }
}
