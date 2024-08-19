package org.dbFrameworks;

import org.dbFrameworks.Facade.FrameworkTestsFacade;
import org.dbFrameworks.Facade.RelationalFrameworkTestsFacade;
import org.dbFrameworks.Menu.FrameworkTestMenu;
import org.dbFrameworks.Menu.RelationalFrameworkMenu;

import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        var menu = new RelationalFrameworkMenu(new RelationalFrameworkTestsFacade(new HibernateManager()),"Hibernate - SQL Server");
        var solrMenu = new FrameworkTestMenu(new FrameworkTestsFacade(new SolrJManager()), "SolrJ");
        System.out.println("Welcome to Database Performance Tester - Java");
        while (true){
            System.out.println("Select framework which you want to test: (press 1-3)");
            System.out.println("1.Hibernate - SQL Server\n3.SolrApi");
            System.out.println("X - Close program\n");
            var input = new Scanner(System.in);
            switch (input.next()){
                case "1":
                   menu.Display();
                    break;
                case "3":
                    solrMenu.Display();
                case "X":
                    return;
            }
        }
    }
}