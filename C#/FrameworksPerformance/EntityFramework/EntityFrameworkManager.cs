using Common;
using Common.Managers;
using Common.Utilites;
using EntityFramework.Models;
using System.Diagnostics;

namespace EntityFramework
{
    public class EntityFrameworkManager : IRelationalFrameworkManager
    {
        private readonly FrameworkPerformanceMtContext _context;
        private readonly Random _random = new();

        public EntityFrameworkManager(FrameworkPerformanceMtContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TestResult SingleRecordSearch(int samplesQuantity)
        {
            var rowCount = _context.Orders.Count() - 1;
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));

            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var number = _random.Next(rowCount) + 1;
                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    var _ = _context.Orders.First(x => x.OrderId == number);

                    sw.Stop();

                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }

            }
            return testResult;
        }

        public TestResult SetOfDataSearch(int samplesQuantity)
        {
            var positionsCount = _context.Positions.Count() - 1;
            var testResult = new TestResult(samplesQuantity, nameof(SetOfDataSearch));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var positionId = _random.Next(positionsCount) + 1;
                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    var _ = _context.Employees.Where(x => x.PositionId.HasValue && x.PositionId.Value == positionId).ToArray();

                    sw.Stop();

                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }

        public TestResult SetOfDataWithIsNullSearch(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult AddRecords(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult EditRecords(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult DeleteRecords(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult SearchTwoRelatedTables(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult SearchFourRelatedTables(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult SearchWithSubQuery(int samplesQuantity)
        {
            throw new NotImplementedException();
        }

        public TestResult SearchWithSubQueryWithJoin(int samplesQuantity)
        {
            throw new NotImplementedException();
        }
    }
}
