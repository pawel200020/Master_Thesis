using Common;
using Common.Managers;
using Common.Utilites;
using EntityFramework.Models;
using System.Diagnostics;

namespace EntityFramework
{
    public class EntityFrameworkManager(FrameworkPerformanceMtContext context) : IRelationalFrameworkManager
    {
        private readonly FrameworkPerformanceMtContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly Random _random = new();
        private const string ProductDescription = "AddedByTest";

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

                    var _ = _context.Orders.Find(number);

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
            var testResult = new TestResult(samplesQuantity, nameof(SetOfDataWithIsNullSearch));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var table = (TableWithNull)_random.Next(Enum.GetNames(typeof(TableWithNull)).Length - 1);
                    Stopwatch sw = new();
                    switch (table)
                    {
                        case TableWithNull.ClientPhone:
                            {
                                sw.Start();
                                var _ = _context.Clients.Where(x => x.PhoneNumber == null).ToArray();
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.ClientCountry:
                            {
                                sw.Start();
                                var _ = _context.Clients.Where(x => x.Country == null).ToArray();
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.EmployeesPhone:
                            {
                                sw.Start();
                                var _ = context.Employees.Where(x => x.PhoneNumber == null);
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.EmployeesPositionId:
                            {
                                sw.Start();
                                var _ = context.Employees.Where(x => x.PositionId == null);
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.Orders:
                            {
                                sw.Start();
                                var _ = context.Orders.Where(x => x.OrderDate == null);
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.Products:
                            {
                                sw.Start();
                                var _ = context.Products.Where(x => x.ProductDescription == null);
                                sw.Stop();
                                break;
                            }
                    }
                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
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
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var name = productNames.ElementAt(_random.Next(productNames.Count()));
                    Stopwatch sw = new();
                    sw.Start();
                    _context.Products.Add(new Product()
                    {
                        ProductName = name,
                        Price = (decimal)21.36,
                        ProductDescription = ProductDescription,
                        Supplier = "None"
                    });
                    _context.SaveChanges();
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }
            RemoveRecordsSilent();
            return testResult;
        }

        public TestResult EditRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(EditRecords));
            var (minId, maxId) = AddRecordsSilent(samplesQuantity);
            var productNames = ReadData("Files/real_product_names.txt");
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var idToEdit = _random.Next(minId, maxId);
                    var newName = productNames.ElementAt(_random.Next(productNames.Count()));
                    Stopwatch sw = new();

                    sw.Start();
                    var recordToEdit = _context.Products.Find(idToEdit);
                    recordToEdit.ProductName = newName;
                    _context.SaveChanges();
                    sw.Stop();

                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }
            RemoveRecordsSilent();
            return testResult;
        }

        public TestResult DeleteRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(DeleteRecords));
            var (minId, maxId) = AddRecordsSilent(samplesQuantity);
            using (var progress = new ProgressBar())
            {
                for (int i = minId; i <= maxId; i++)
                {
                    Stopwatch sw = new();

                    sw.Start();
                    var recordToRemove = _context.Products.Find(i);
                    _context.Products.Remove(recordToRemove);
                    _context.SaveChanges();
                    sw.Stop();

                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }
            return testResult;
        }

        public TestResult SearchTwoRelatedTables(int samplesQuantity)
        {
            var positionsCount = _context.Positions.Count() - 1;
            var testResult = new TestResult(samplesQuantity, nameof(SearchTwoRelatedTables));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var positionToFind = _context.Positions.Find(_random.Next(positionsCount) + 1);
                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    var _ = _context.Employees.Where(x => x.Position != null && x.Position.PositionName == positionToFind!.PositionName).ToArray();

                    sw.Stop();

                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }

        public TestResult SearchFourRelatedTables(int samplesQuantity)
        {
            var positionsCount = _context.Positions.Count() - 1;
            var storesCount = _context.Stores.Count() - 1;
            var testResult = new TestResult(samplesQuantity, nameof(SearchFourRelatedTables));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var positionToFind = _context.Positions.Find(_random.Next(positionsCount) + 1);
                    var storeToFind = _context.Stores.Find(_random.Next(storesCount) + 1);
                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    var _ = _context.Orders.Where(x =>
                        storeToFind != null && x.Employee.Position != null && x.Employee.Position.PositionName == positionToFind.PositionName && x.Store.Country == storeToFind.Country).ToArray();
                    sw.Stop();

                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }
        /// <summary>
        /// Select * from [Positions] where [PositionId] not in (Select DISTINCT [PositionId] from Employees)
        /// Select * from [Stores] where [StoreId] not in (Select DISTINCT [StoreId] from Orders)
        /// </summary>
        /// <param name="samplesQuantity"></param>
        /// <returns></returns>
        public TestResult SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SearchRecordsWhichDoesNotHaveConnection));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    Stopwatch sw = new Stopwatch();

                    if (i % 2 == 0)
                    {
                        sw.Start();
                        var _ = _context.Positions.Where(x => !x.Employees.Any()).ToArray();
                        sw.Stop();
                    }
                    else
                    {
                        sw.Start();
                        var _ = _context.Stores.Where(x => !x.Orders.Any()).ToArray();
                        sw.Stop();
                    }
                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }
        /// <summary>
        /// Select * from Orders where ClientId in (Select ClientId from Clients where Country = 'X')
        /// </summary>
        /// <param name="samplesQuantity"></param>
        /// <returns></returns>
        public TestResult SearchWithSubQuery(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SearchWithSubQuery));
            var countries = ReadData("Files/Country.txt");
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var countryToSearch = countries.ElementAt(_random.Next(countries.Count()));
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var _ = _context.Orders.Where(x => x.Client.Country == countryToSearch).ToArray();
                    sw.Stop();
                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
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

        private (int minId, int maxId) AddRecordsSilent(int docsToAdd)
        {
            var productNames = ReadData("Files/real_product_names.txt");
            var minId = 0;
            var maxId = 0;
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < docsToAdd; i++)
                {
                    var name = productNames.ElementAt(_random.Next(productNames.Count()));

                    var item = _context.Products.Add(new Product()
                    {
                        ProductName = name,
                        Price = (decimal)21.36,
                        ProductDescription = ProductDescription,
                        Supplier = "None"
                    });
                    _context.SaveChanges();
                    if (i == 0)
                        minId = item.Entity.ProductId;
                    if (i == docsToAdd - 1)
                        maxId = item.Entity.ProductId;
                    progress.Report((double)i / docsToAdd);
                }
            }
            return (minId, maxId);
        }

        public TestResult RemoveRelatedRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(RemoveRelatedRecords));
            var clientsCount = _context.Clients.Count() - 1;
            var employeesCount = _context.Employees.Count() - 1;

            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var client = _context.Clients.Find(_random.Next(clientsCount) + 1);
                    var employee = _context.Employees.Find(_random.Next(employeesCount) + 1);
                    var addedStore = _context.Stores.Add(new Store()
                    {
                        Address = "Norymberska 1 30-412 Krakow",
                        Country = "Poland",
                        Orders = new List<Order>()
                        {
                            new()
                            {
                                Client = client!,
                                Employee = employee!,
                                OrderDate = DateTime.Now,
                                OrderDetails = OrderXml,
                                TotalCost = 120
                            }
                        }
                    });
                    _context.SaveChanges();
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    _context.Stores.Remove(addedStore.Entity);
                    _context.SaveChanges();
                    sw.Stop();
                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);

                }
            }
            return testResult;
        }

        private void RemoveRecordsSilent()
        {
            var itemsToRemove = _context.Products.Where(x => x.ProductDescription == ProductDescription);
            _context.RemoveRange(itemsToRemove);
            _context.SaveChanges();
        }

        private const string OrderXml = @"<Order xmlns=""urn:OrdersInfoNamespace"">
  <Product id=""0"">
    <Name>rock</Name>
    <Quantity>669</Quantity>
  </Product>
  <Product id=""1"">
    <Name>toilet</Name>
    <Quantity>837</Quantity>
  </Product>
  <Product id=""2"">
    <Name>newspaper</Name>
    <Quantity>38</Quantity>
  </Product>
</Order>";
    }
}
