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
        private IEnumerable<string>? _productNamesCached;

        private IEnumerable<string> ProductNames
        {
            get
            {
                if (_productNamesCached != null) return _productNamesCached;

                _productNamesCached = ReadData("Files/real_product_names.txt");
                return _productNamesCached;
            }
        }

        public TestResult SingleRecordSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));
            using (var session = _contextFactory.OpenSession())
            {
                var rowCount = session.Query<Employee>().Count() - 1;

                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var number = _random.Next(rowCount) + 1;
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var _ = session.Get<Employee>(number);
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
                        var table = (TableWithNull)_random.Next(Enum.GetNames(typeof(TableWithNull)).Length - 1);
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
                        progress.Report((double)i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }
            }
            return testResult;
        }

        public TestResult AddRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(AddRecords));
            var productNames = ProductNames;
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
                            Price = (decimal)21.36,
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
                        progress.Report((double)i / samplesQuantity);
                    }
                }
            }
            RemoveRecordsSilent();
            return testResult;
        }

        private void RemoveRecordsSilent()
        {
            var productNames = ProductNames;
            using (var session = _contextFactory.OpenSession())
            {
                var recordsToDelete =
                    session.Query<Product>().Where(x => x.ProductDescription == ProductDescription);
                var count = recordsToDelete.Count();
                var i = 0;
                using (var progress = new ProgressBar())
                {
                    foreach (var product in recordsToDelete)
                    {
                        using (ITransaction tx = session.BeginTransaction())
                        {
                            session.Delete(product);
                            tx.Commit();
                        }
                        progress.Report((double)i / count);
                        i++;
                    }
                }
            }
        }

        public TestResult EditRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(EditRecords));
            var (minId, maxId) = AddRecordsSilent(samplesQuantity);
            var productNames = ProductNames;
            using (var progress = new ProgressBar())
            {
                using (var session = _contextFactory.OpenSession())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var idToEdit = _random.Next(minId, maxId);
                        var newName = productNames.ElementAt(_random.Next(productNames.Count()));
                        Stopwatch sw = new();
                        sw.Start();
                        var recordToEdit = session.Get<Product>(idToEdit);
                        recordToEdit.ProductName = newName;
                        using (ITransaction tx = session.BeginTransaction())
                        {
                            session.Update(recordToEdit);
                            tx.Commit();
                        }
                        sw.Stop();
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                        progress.Report((double)i / samplesQuantity);
                    }
                }
            }
            RemoveRecordsSilent();
            return testResult;
        }

        public TestResult DeleteRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(DeleteRecords));
            var (minId, maxId) = AddRecordsSilent(samplesQuantity);
            var j = 0;
            using (var progress = new ProgressBar())
            {
                using (var session = _contextFactory.OpenSession())
                {
                    for (var i = minId; i <= maxId; i++)
                    {
                        Stopwatch sw = new();
                        sw.Start();
                        var recordToRemove = session.Get<Product>(i);
                        using (ITransaction tx = session.BeginTransaction())
                        {
                            session.Delete(recordToRemove);
                            tx.Commit();
                        }
                        sw.Stop();
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                        progress.Report((double)j / samplesQuantity);
                        j++;
                    }
                }
            }
            return testResult;
        }

        public TestResult SearchTwoRelatedTables(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SearchTwoRelatedTables));
            using (var session = _contextFactory.OpenSession())
            {
                var positionsCount = session.Query<Position>().Count() - 1;
                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var idToFind = _random.Next(positionsCount) + 1;
                        var positionToFind = session.Get<Position>(idToFind);
                        Stopwatch sw = new Stopwatch();

                        sw.Start();
                        var _ = session.Query<Employee>().Where(x =>
                            x.Position != null && x.Position.PositionName == positionToFind.PositionName).ToArray();
                        sw.Stop();

                        progress.Report((double)i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }
            }

            return testResult;
        }

        public TestResult SearchFourRelatedTables(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SearchFourRelatedTables));
            using (var session = _contextFactory.OpenSession())
            {
                var positionsCount = session.Query<Position>().Count() - 1;
                var storesCount = session.Query<Store>().Count() - 1;

                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var positionToFind = session.Get<Position>(_random.Next(positionsCount) + 1);
                        var storeToFind = session.Get<Store>(_random.Next(storesCount) + 1);
                        Stopwatch sw = new Stopwatch();

                        sw.Start();

                        var _ = session.Query<Order>().Where(x =>
                            x.Employee.Position != null &&
                            x.Employee.Position.PositionName == positionToFind.PositionName &&
                            x.Store.Country == storeToFind.Country).ToArray();
                        sw.Stop();

                        progress.Report((double)i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }
            }

            return testResult;
        }

        public TestResult SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SearchRecordsWhichDoesNotHaveConnection));
            using (var session = _contextFactory.OpenSession())
            {
                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        Stopwatch sw = new Stopwatch();

                        if (i % 2 == 0)
                        {
                            sw.Start();
                            var usedPositions = session.Query<Employee>().Where(x => x.Position != null)
                                .Select(x => x.Position.PositionId).Distinct();
                            var _ = session.Query<Position>().Where(x => !usedPositions.Contains(x.PositionId))
                                .ToArray();
                            sw.Stop();
                        }
                        else
                        {
                            sw.Start();
                            var usedStores = session.Query<Order>().Select(x => x.Store.StoreId).Distinct();
                            var _ = session.Query<Store>().Where(x => !usedStores.Contains(x.StoreId)).ToArray();
                            sw.Stop();
                        }

                        progress.Report((double)i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }

                return testResult;
            }
        }

        public TestResult SearchWithSubQuery(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SearchWithSubQuery));
            var countries = ReadData("Files/Country.txt");
            using (var session = _contextFactory.OpenSession())
            {
                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var countryToSearch = countries.ElementAt(_random.Next(countries.Count()));
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var _ = session.Query<Order>().Where(x => x.Client.Country == countryToSearch).ToArray();
                        sw.Stop();
                        progress.Report((double) i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    }
                }
            }

            return testResult;
        }

        public TestResult RemoveRelatedRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(RemoveRelatedRecords));
            using (var session = _contextFactory.OpenSession())
            {
                var clientsCount = session.Query<Client>().Count() - 1;
                var employeesCount = session.Query<Employee>().Count() - 1;

                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < samplesQuantity; i++)
                    {
                        var client = session.Get<Client>(_random.Next(clientsCount) + 1);
                        var employee = session.Get<Employee>(_random.Next(employeesCount) + 1);

                        var store = new Store()
                        {
                            Address = "Norymberska 1 30-412 Krakow",
                            Country = "Poland",
                        };
                        var order = new Order()
                        {
                            Client = client,
                            Employee = employee,
                            OrderDate = DateTime.Now,
                            OrderDetails = OrderXml,
                            Store = store
                        };
                        using (ITransaction tx = session.BeginTransaction())
                        {
                            session.Save(store);
                            session.Save(order);
                            tx.Commit();
                        }

                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        using (ITransaction tx = session.BeginTransaction())
                        {
                            session.Delete(order);
                            tx.Commit();
                        }
                        sw.Stop();
                        progress.Report((double)i / samplesQuantity);
                        testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);

                    }
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
            var productNames = ProductNames;
            var minId = 0;
            var maxId = 0;
            using (var progress = new ProgressBar())
            {
                using (var session = _contextFactory.OpenSession())
                {
                    for (int i = 0; i < docsToAdd; i++)
                    {
                        var name = productNames.ElementAt(_random.Next(productNames.Count()));
                        Stopwatch sw = new();
                        sw.Start();

                        var product = new Product()
                        {
                            ProductName = name,
                            Price = (decimal)21.36,
                            ProductDescription = ProductDescription,
                            Supplier = "None"
                        };
                        using (ITransaction tx = session.BeginTransaction())
                        {
                            session.Save(product);
                            tx.Commit();
                        }
                        sw.Stop();
                        progress.Report((double)i / docsToAdd);
                        if (i == 0)
                            minId = product.ProductId;
                        if (i == docsToAdd - 1)
                            maxId = product.ProductId;
                    }
                }
            }
            return (minId, maxId);
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
