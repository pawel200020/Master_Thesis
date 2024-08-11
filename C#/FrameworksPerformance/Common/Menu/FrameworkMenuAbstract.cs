namespace Common.Menu
{
    public abstract class FrameworkMenuAbstract : IFrameworkMenu
    {
        protected abstract string Operations { get; }
        public string FrameworkName { get; init; }

        protected void DisplayResult(string result)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Result\n----------------------\n{result}\n----------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        protected abstract string GetResult(ConsoleKeyInfo input, int samples); 

        public void Display()
        {
            while (true)
            {
                Console.WriteLine(string.Format(Operations, FrameworkName));
                var input = Console.ReadKey();
                Console.WriteLine();

                if(input.Key == ConsoleKey.X) 
                    return;

                Console.Write($"\nNumber of repetitions: ");
                var samples = Console.ReadLine();
                Console.WriteLine("");
                if (!int.TryParse(samples, out var number))
                {
                    Console.WriteLine("not a number");
                    continue;
                }
                DisplayResult(GetResult(input, number));
            }
        }
    }
}
