using System.Diagnostics;
using Common;
using Common.Managers;
using Common.Utilites;
using NHibernate;
using NHibernateModels;

namespace NHibernateSql
{
    public class NHibernateManager(ISessionFactory contextFactory) : IRelationalFrameworkManager
    {
        private readonly ISessionFactory _contextFactory = contextFactory;
        private readonly Random _random = new();
        private const string ProductDescription = "AddedByTest";

        public TestResult SingleRecordSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));
            using (var session = _contextFactory.OpenSession())
            {
                var rowCount = session.Query<Employee>().Count() -1;
                
                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var number = _random.Next(rowCount) + 1;
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var _ = session.Query<Employee>().First(x => x.EmployeeId == number);
                        sw.Stop();
                        progress.Report((double)i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }
            }
            return testResult;
        }

        public TestResult SetOfDataSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));
            using (var session = _contextFactory.OpenSession())
            {
                var positionsCount = session.Query<Position>().Count() - 1;

                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var positionId = _random.Next(positionsCount) + 1;
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var _ = session.Query<Employee>().Where(x => x.Position != null && x.Position.PositionId == positionId).ToArray();
                        sw.Stop();
                        progress.Report((double)i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }
            }
            return testResult;
        }

        public TestResult SetOfDataWithIsNullSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SetOfDataWithIsNullSearch));
            using (var progress = new ProgressBar())
            {
                using (var session = _contextFactory.OpenSession())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var table = (TableWithNull) _random.Next(Enum.GetNames(typeof(TableWithNull)).Length - 1);
                        Stopwatch sw = new();
                        switch (table)
                        {
                            case TableWithNull.ClientPhone:
                            {
                                sw.Start();
                                var _ = session.Query<Client>().Where(x => x.PhoneNumber == null).ToArray();
                                sw.Stop();
                                break;
                            }
                            case TableWithNull.ClientCountry:
                            {
                                sw.Start();
                                var _ = session.Query<Client>().Where(x => x.Country == null).ToArray();
                                sw.Stop();
                                break;
                            }
                            case TableWithNull.EmployeesPhone:
                            {
                                sw.Start();
                                var _ = session.Query<Employee>().Where(x => x.PhoneNumber == null).ToArray();
                                sw.Stop();
                                break;
                            }
                            case TableWithNull.EmployeesPositionId:
                            {
                                sw.Start();
                                var _ = session.Query<Employee>().Where(x => x.Position == null).ToArray();
                                sw.Stop();
                                break;
                            }
                            case TableWithNull.Orders:
                            {
                                sw.Start();
                                var _ = session.Query<Order>().Where(x => x.OrderDate == null).ToArray();
                                sw.Stop();
                                break;
                            }
                            case TableWithNull.Products:
                            {
                                sw.Start();
                                var _ = session.Query<Product>().Where(x => x.ProductDescription == null).ToArray();
                                sw.Stop();
                                break;
                            }
                        }
                        progress.Report((double) i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }
            }
            return testResult;
        }

        public TestResult AddRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(AddRecords));
            var productNames = ReadData("Files/real_product_names.txt");
            using (var progress = new ProgressBar())
            {
                using (var session = _contextFactory.OpenSession())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var name = productNames.ElementAt(_random.Next(productNames.Count()));
                        Stopwatch sw = new();
                        sw.Start();

                        var product = new Product()
                        {
                            ProductName = name,
                            Price = (decimal) 21.36,
                            ProductDescription = ProductDescription,
                            Supplier = "None"
                        };
                        using (ITransaction tx = session.BeginTransaction())
                        {
                            session.Save(product);
                            tx.Commit();
                        }
                        sw.Stop();
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                        progress.Report((double) i / samplesQuantity);
                    }
                }
            }
            RemoveRecordsSilent();
            return testResult;
        }

        private void RemoveRecordsSilent()
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

        public TestResult SearchThreeRelatedTables(int samplesQuantity)
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

        private IEnumerable<string> ReadData(string path)
        {
            var result = new List<string>();
            using StreamReader sr = new StreamReader(path);
            var line = sr.ReadLine();
            while (line is not null)
            {
                result.Add(line);
                line = sr.ReadLine();
            }
            return result;
        }
    }
}
