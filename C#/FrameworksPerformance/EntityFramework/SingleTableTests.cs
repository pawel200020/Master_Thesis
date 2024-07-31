using Common;
using EntityFramework.Models;
using System.Diagnostics;

namespace EntityFramework
{
    public class SingleTableTests
    {
        private readonly FrameworkPerformanceMtContext _context;
        private readonly Random _random = new();

        public SingleTableTests(FrameworkPerformanceMtContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TestResult SearchSingleRecord(int repetitions)
        {
            var rowCount = _context.Orders.Count();
            var testResult = new TestResult(repetitions, nameof(SearchSingleRecord));
            for (int i = 0; i < repetitions; i++)
            {
                var number = _random.Next(rowCount);
                Stopwatch sw = new Stopwatch();

                sw.Start();

                var _ = _context.Orders.First(x => x.OrderId == number);

                sw.Stop();

                testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                Console.WriteLine("Elapsed={0}", sw.Elapsed.TotalMilliseconds);
            }
            return testResult;
        }
    }

}
