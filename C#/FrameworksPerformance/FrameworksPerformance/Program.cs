using System.Configuration;
using Common.Facades;
using Common.Menu;
using Common.Utilites;
using EntityFramework;
using EntityFrameworkSqlite;
using NHibernateSql;
using SolrEngine;
using static SolrNet.StartOrCursor;

var jsonManager = new JsonManager();
var efMenu = new RelationalFrameworkMenu("Entity Framework - SQL Server",
    new RelationalFrameworkTestsFacade(new EntityFrameworkManagerFactory().Create(), jsonManager));
var efSqliteMenu = new RelationalFrameworkMenu("Entity Framework - SQLite",
    new RelationalFrameworkTestsFacade(new EntityFrameworkSqliteManagerFactory().Create(),jsonManager));
var nHibernateMenu = new RelationalFrameworkMenu("NHibernate - SQL Server",
    new RelationalFrameworkTestsFacade(new NHibernateManagerFactory().Create(), jsonManager));

var solrMenu = new FrameworkMenu("Solr", new FrameworkTestFacade(new SolrManager(), jsonManager));

var sAttr = ConfigurationManager.AppSettings.Get("Key0");

Console.WriteLine("Welcome to Database Performance Tester - C#");
while (true)
{
    Console.WriteLine("Select framework which you want to test: (press 1-3)");
    Console.WriteLine("1.Entity Framework - SQL Server\n2.Entity Framework - SQLite\n3.NHibernate\n4.SolrNet");
    Console.WriteLine("X - Close program\n");
    var input = Console.ReadKey();
    switch (input.Key)
    {
        case ConsoleKey.D1:
        {
            efMenu.Display();
            break;
        }
        case ConsoleKey.D2:
        {
            efSqliteMenu.Display();
            break;
        }
        case ConsoleKey.D3:
        {
            nHibernateMenu.Display();
            break;
        }
        case ConsoleKey.D4:
        {
            solrMenu.Display();
            break;
        }
        case ConsoleKey.X:
        {
            return 0;
        }
        default:
        {
            Console.WriteLine("Wrong Command");
            break;
        }
    }

}