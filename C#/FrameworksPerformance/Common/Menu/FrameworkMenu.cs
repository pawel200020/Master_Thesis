using Common.Facades;

namespace Common.Menu
{
    public class FrameworkMenu : FrameworkMenuAbstract
    {
        private readonly IFrameworkTestsFacade _frameworkTestsFacade;
        public FrameworkMenu(string frameworkName, IFrameworkTestsFacade frameworkTestsFacade)
        {
            _frameworkTestsFacade = frameworkTestsFacade ?? throw new ArgumentNullException(nameof(frameworkTestsFacade));
            FrameworkName = frameworkName;
        }

        protected override string Operations => @"
Selected framework - {0}
Choose operation:

A - Run All Tests and save to file

Single table tests:
B - Single record search
C - Set of records search
D - Set of records search with isNull command
E - Adding single records
F - Edit single record
G - Delete single record
";
        protected virtual string NumberOfRepetitions => "\nNumber of repetitions: ";
        protected override string GetResult(ConsoleKeyInfo input, int samples) => input.Key switch
        {
            ConsoleKey.A => _frameworkTestsFacade.RunAllTests(samples, "test"),
            ConsoleKey.B => _frameworkTestsFacade.SingleRecordSearch(samples),
            ConsoleKey.C => _frameworkTestsFacade.SetOfDataSearch(samples),
            ConsoleKey.D => _frameworkTestsFacade.SetOfDataWithIsNullSearch(samples),
            ConsoleKey.E => _frameworkTestsFacade.AddRecords(samples),
            ConsoleKey.F => _frameworkTestsFacade.EditRecords(samples),
            ConsoleKey.G => _frameworkTestsFacade.DeleteRecords(samples),
            _ => "Wrong Command"
        };
    }
}
