package org.dbFrameworks;

import org.dbFrameworks.Facade.RelationalFrameworkTestsFacade;
import org.dbFrameworks.Menu.RelationalFrameworkMenu;

import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        var menu = new RelationalFrameworkMenu(new RelationalFrameworkTestsFacade(new HibernateManager()),"Hibernate - SQL Server");
        System.out.println("Welcome to Database Performance Tester - Java");
        while (true){
            System.out.println("Select framework which you want to test: (press 1-3)");
            System.out.println("1.Hibernate - SQL Server\n2.Hibernate - SQLite\n3.SolrApi");
            System.out.println("X - Close program\n");
            var input = new Scanner(System.in);
            switch (input.nextInt()){
                case 1:
                    menu.Display();
                    break;
                case 'X':
                    return;
            }
        }
    }
}