using Common.Facades;
using Common.Menu;
using Common.Utilites;
using EntityFramework;

var jsonManager = new JsonManager();
var efMenu = new RelationalFrameworkMenu("Entity Framework",
    new RelationalFrameworkTestsFacade(new EntityFrameworkManagerFactory().Create(), jsonManager));

Console.WriteLine("Welcome to Database Performance Tester - C#");
while (true)
{
    Console.WriteLine("Select framework which you want to test: (press 1-3)");
    Console.WriteLine("1.Entity Framework\n2.NHibernate\n3.SolrNet");
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
            break;
        }
        case ConsoleKey.D3:
        {
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