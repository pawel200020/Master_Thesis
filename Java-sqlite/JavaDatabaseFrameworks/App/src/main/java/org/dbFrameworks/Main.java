package org.dbFrameworks;

import org.dbFrameworks.Facade.RelationalFrameworkTestsFacade;
import org.dbFrameworks.Menu.RelationalFrameworkMenu;

import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        var sqliteMenu = new RelationalFrameworkMenu(new RelationalFrameworkTestsFacade(new HibernateSqliteManager()),"Hibernate - SQLite");
        System.out.println("Welcome to Database Performance Tester - Java");
        while (true){
            System.out.println("Select framework which you want to test: (press 1)");
            System.out.println("2.Hibernate - SQLite\n");
            System.out.println("X - Close program\n");
            var input = new Scanner(System.in);
            switch (input.next()){
                case "2":
                    sqliteMenu.Display();
                    break;
                case "X":
                    return;
            }
        }
    }
}