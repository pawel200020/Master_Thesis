using Common.Facades;

namespace Common.Menu
{
    public class RelationalFrameworkMenu : FrameworkMenuAbstract
    {
        private readonly IRelationalFrameworkTestsFacade _frameworkTestsFacade;

        public RelationalFrameworkMenu(string frameworkName, IRelationalFrameworkTestsFacade frameworkTestsFacade)
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

Multiple table tests:
H - Searching in two connected tables
I - Searching in four connected tables
J - Searching for records which does not have connection
K - Searching with subquery

X - Main menu
";

        protected override string GetResult(ConsoleKeyInfo input, int samples) => input.Key switch
        {
            ConsoleKey.A => _frameworkTestsFacade.RunAllTests(samples,"test"),
            ConsoleKey.B => _frameworkTestsFacade.SingleRecordSearch(samples),
            ConsoleKey.C => _frameworkTestsFacade.SetOfDataSearch(samples),
            ConsoleKey.D => _frameworkTestsFacade.SetOfDataWithIsNullSearch(samples),
            ConsoleKey.E => _frameworkTestsFacade.AddRecords(samples),
            ConsoleKey.F => _frameworkTestsFacade.EditRecords(samples),
            ConsoleKey.G => _frameworkTestsFacade.DeleteRecords(samples),
            ConsoleKey.H => _frameworkTestsFacade.SearchTwoRelatedTables(samples),
            ConsoleKey.I => _frameworkTestsFacade.SearchFourRelatedTables(samples),
            ConsoleKey.J => _frameworkTestsFacade.SearchRecordsWhichDoesNotHaveConnection(samples),
            ConsoleKey.K => _frameworkTestsFacade.SearchWithSubQuery(samples),
            _ => "Wrong Command"
        };

    }
}
