package org.dbFrameworks.Menu;

import org.dbFrameworks.Facade.IFrameworkTestsFacade;

import java.util.Scanner;

public class FrameworkTestMenu {
    private final IFrameworkTestsFacade _frameworkTestsFacade;
    private final String _frameworkName;

    public static final String TEXT_RESET  = "\u001B[0m";
    public static final String TEXT_GREEN  = "\u001B[32m";

    protected String Operations(String frameworkName) { return String.format("""
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

    X - Main menu
""",frameworkName);}


    public FrameworkTestMenu(IFrameworkTestsFacade frameworkTestsFacade, String frameworkName) {
        _frameworkTestsFacade = frameworkTestsFacade;
        _frameworkName = frameworkName;
    }

    private void DisplayResult(String result){
        System.out.println(TEXT_GREEN + "Result\n----------------------\n"+result+"\n----------------------");
        System.out.println(TEXT_RESET+"Press any key to continue...");
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
                    DisplayResult(_frameworkTestsFacade.RunAllTests(repeititions,"test.txt"));
                    break;
                case "B":
                    DisplayResult(_frameworkTestsFacade.SingleRecordSearch(repeititions));
                    break;
                case "C":
                    DisplayResult(_frameworkTestsFacade.SetOfDataSearch(repeititions));
                    break;
                case "D":
                    DisplayResult(_frameworkTestsFacade.SetOfDataWithIsNullSearch(repeititions));
                    break;
                case "E":
                    DisplayResult(_frameworkTestsFacade.AddRecords(repeititions));
                    break;
                case "F":
                    DisplayResult(_frameworkTestsFacade.EditRecords(repeititions));
                    break;
                case "G":
                    DisplayResult(_frameworkTestsFacade.DeleteRecords(repeititions));
                    break;
                case "X":
                    return;
            }
        }
    }
}
