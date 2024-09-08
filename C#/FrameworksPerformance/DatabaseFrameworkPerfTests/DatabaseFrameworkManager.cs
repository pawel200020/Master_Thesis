using Common;
using Common.Managers;
using Common.Utilites;
using DatabaseFramework;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using DatabaseFramework.Models;
using DatabaseFramework.SqlParams;
using DatabaseFramework.Utilities;
using MatchType = DatabaseFramework.SqlParams.MatchType;

namespace DatabaseFrameworkPerfTests
{
    public class DatabaseFrameworkManager(string connectionString) : IRelationalFrameworkManager
    {
        private readonly FrameworkRepositoryProvider _frameworkProviders = new(connectionString);
        private readonly Random _random = new();
        private const string ProductDescription = "AddedByTest";

        public TestResult SingleRecordSearch(int samplesQuantity)
        {
            var ordersProvider = _frameworkProviders.GetOrdersRepository();

            var rowCount = ordersProvider.CountRecords(new WhereParameter());
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));

            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var number = _random.Next(rowCount) + 1;
                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    var _ = ordersProvider.GetById(number);

                    sw.Stop();

                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }

            }
            return testResult;
        }

        public TestResult SetOfDataSearch(int samplesQuantity)
        {
            var positionsCount = _frameworkProviders.GetPositionRepository().CountRecords(new WhereParameter());
            var testResult = new TestResult(samplesQuantity, nameof(SetOfDataSearch));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var positionId = _random.Next(positionsCount) + 1;
                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                    {
                        {(nameof(Employee),nameof(Employee.PositionId)),(MatchType.Number,positionId.ToString())}
                    });
                    var _ = _frameworkProviders.GetEmployeeRepository().FilterFields(new WhereParameter(parameters, JoinType.Or));

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
                                var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                                {
                                    {(nameof(Client),nameof(Client.PhoneNumber)),(MatchType.IsNull,null)}
                                });
                                var _ = _frameworkProviders.GetClientRepository()
                                    .FilterFields(new WhereParameter(parameters, JoinType.Or));
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.ClientCountry:
                            {
                                sw.Start();
                                var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                                {
                                    {(nameof(Client),nameof(Client.Country)),(MatchType.IsNull,null)}
                                });
                                var _ = _frameworkProviders.GetClientRepository()
                                    .FilterFields(new WhereParameter(parameters, JoinType.Or));
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.EmployeesPhone:
                            {
                                sw.Start();
                                var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                                {
                                    {(nameof(Employee),nameof(Employee.PhoneNumber)),(MatchType.IsNull,null)}
                                });
                                var _ = _frameworkProviders.GetEmployeeRepository()
                                    .FilterFields(new WhereParameter(parameters, JoinType.Or));
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.EmployeesPositionId:
                            {
                                sw.Start();
                                var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                                {
                                    {(nameof(Employee),nameof(Employee.PositionId)),(MatchType.IsNull,null)}
                                });
                                var _ = _frameworkProviders.GetEmployeeRepository()
                                    .FilterFields(new WhereParameter(parameters, JoinType.Or));
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.Orders:
                            {
                                sw.Start();
                                var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                                {
                                    {(nameof(Order),nameof(Order.OrderDate)),(MatchType.IsNull,null)}
                                });
                                var _ = _frameworkProviders.GetOrdersRepository()
                                    .FilterFields(new WhereParameter(parameters, JoinType.Or));
                                sw.Stop();
                                break;
                            }
                        case TableWithNull.Products:
                            {
                                sw.Start();
                                var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                                {
                                    {(nameof(Product),nameof(Product.ProductDescription)),(MatchType.IsNull,null)}
                                });
                                var _ = _frameworkProviders.GetPositionRepository()
                                    .FilterFields(new WhereParameter(parameters, JoinType.Or));
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
            var addedIds = new List<int>();
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var name = productNames.ElementAt(_random.Next(productNames.Count()));
                    Stopwatch sw = new();
                    sw.Start();
                    addedIds.Add(_frameworkProviders.GetProductRepository().AddRow(new Product()
                    {
                        ProductName = name,
                        Price = (decimal)21.36,
                        ProductDescription = ProductDescription,
                        Supplier = "None"
                    }));
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }
            RemoveRecordsSilent(addedIds);
            return testResult;
        }


        public TestResult EditRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(EditRecords));
            var addedIds = AddRecordsSilent(samplesQuantity);
            var productNames = ReadData("Files/real_product_names.txt");
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var idToEdit = addedIds.ElementAt(_random.Next(addedIds.Count()));
                    var newName = productNames.ElementAt(_random.Next(productNames.Count()));
                    Stopwatch sw = new();

                    sw.Start();
                    var recordToEdit = _frameworkProviders.GetProductRepository().GetById(idToEdit);
                    recordToEdit.ProductName = newName;
                    _frameworkProviders.GetProductRepository().EditRow(recordToEdit);
                    sw.Stop();

                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double) i / samplesQuantity);
                }
            }
            RemoveRecordsSilent(addedIds);
            return testResult;
        }

        public TestResult DeleteRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(DeleteRecords));
            var addedIds = AddRecordsSilent(samplesQuantity);
            int i = 0;
            using (var progress = new ProgressBar())
            {
                foreach (var addedId in addedIds)
                {
                    Stopwatch sw = new();
                    sw.Start();
                    _frameworkProviders.GetProductRepository().DeleteRow(addedId);
                    sw.Stop();
                    progress.Report((double)i / addedIds.Count());
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    i++;
                }
            }

            return testResult;
        }

        public TestResult SearchTwoRelatedTables(int samplesQuantity)
        {
            var positionsCount = _frameworkProviders.GetPositionRepository().CountRecords(new WhereParameter());
            var testResult = new TestResult(samplesQuantity, nameof(SearchTwoRelatedTables));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var positionToFind = _frameworkProviders.GetPositionRepository()
                        .GetById(_random.Next(positionsCount) + 1);
                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                    {
                        {(nameof(Position),nameof(Position.PositionName)),(MatchType.Exact,positionToFind!.PositionName)}
                    });
                    var _ = _frameworkProviders.GetEmployeeRepository().FilterFields(new WhereParameter(parameters, JoinType.Or));
                    sw.Stop();

                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }

        public TestResult SearchFourRelatedTables(int samplesQuantity)
        {
            var positionsCount = _frameworkProviders.GetPositionRepository().CountRecords(new WhereParameter());
            var storesCount = _frameworkProviders.GetStoreRepository().CountRecords(new WhereParameter());
            var testResult = new TestResult(samplesQuantity, nameof(SearchTwoRelatedTables));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var positionToFind = _frameworkProviders.GetPositionRepository()
                        .GetById(_random.Next(positionsCount) + 1);

                    var storeToFind = _frameworkProviders.GetStoreRepository()
                        .GetById(_random.Next(storesCount) + 1);
                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                    {
                        {(nameof(Position),nameof(Position.PositionName)),(MatchType.Exact,positionToFind!.PositionName)},
                        {(nameof(Store),nameof(Store.Country)),(MatchType.Exact,storeToFind!.Country)}
                    });
                    var _ = _frameworkProviders.GetOrdersRepository().FilterFields(new WhereParameter(parameters, JoinType.And));
                    sw.Stop();

                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }

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
                        var command = @"Select p.PositionId as Position_PositionId
      ,p.PositionName as Position_PositionName
      ,p.CarParkingPlace as Position_CarParkingPlace
      ,p.PositionBonus as Position_Bonus
from Positions as p Where p.PositionId not in(select Distinct p.positionId FROM Employees as e INNER JOIN positions as p on e.PositionId = p.PositionId)
";
                        var _ = _frameworkProviders.GetPositionRepository().RunCustomQuery(new SqlCommand(command));
                        sw.Stop();
                    }
                    else
                    {
                        sw.Start();
                        var command = @"Select s.StoreId as Store_StoreId
      ,s.Address as Store_Address
      ,s.Country as Store_Country
  FROM Stores as s where s.StoreId not in (Select Distinct s.StoreId from Orders as o Inner join Stores as s on s.StoreId = o.StoreId)
";
                        var _ = _frameworkProviders.GetStoreRepository().RunCustomQuery(new SqlCommand(command));
                        sw.Stop();
                    }
                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }

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
                    var parameters = new ReadOnlyDictionary<(string className, string fieldName), (MatchType matchType, string? value)>(new Dictionary<(string className, string fieldName), (MatchType matchType, string? value)>()
                    {
                        {(nameof(Client),nameof(Client.Country)),(MatchType.Exact,countryToSearch)},
                    });
                    var _ = _frameworkProviders.GetOrdersRepository()
                        .FilterFields(new WhereParameter(parameters, JoinType.Or));
                    sw.Stop();
                    progress.Report((double)i / samplesQuantity);
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                }
            }
            return testResult;
        }

        public TestResult RemoveRelatedRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(RemoveRelatedRecords));
            var clientsCount = _frameworkProviders.GetClientRepository().CountRecords(new WhereParameter());
            var employeesCount = _frameworkProviders.GetEmployeeRepository().CountRecords(new WhereParameter());

            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var client = _frameworkProviders.GetClientRepository().GetById(_random.Next(clientsCount) + 1);
                    var employee = _frameworkProviders.GetEmployeeRepository().GetById(_random.Next(employeesCount) + 1);
                    var orderId = _frameworkProviders.GetOrdersRepository().AddRow(new Order()
                    {
                        Client = client,
                        Employee = employee,
                        ClientId = client.ClientId,
                        EmployeeId = employee.EmployeeId,
                        OrderDate = DateTime.Now,
                        OrderDetails = OrderXml,
                        TotalCost = 123,
                        StoreId = -1,
                        Store = new Store()
                        {
                            Address = "Norymberska 1 30-412 Krakow",
                            Country = "Poland",
                            StoreId = -1
                        },
                        OrderId = -1
                    });
                    var storeId = _frameworkProviders.GetOrdersRepository().GetById(orderId)!.StoreId;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    _frameworkProviders.GetStoreRepository().DeleteRow(storeId);
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

        private IEnumerable<int> AddRecordsSilent(int samplesQuantity)
        {
            var productNames = ReadData("Files/real_product_names.txt");
            var addedIds = new List<int>();
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var name = productNames.ElementAt(_random.Next(productNames.Count()));
                    Stopwatch sw = new();
                    sw.Start();
                    addedIds.Add(_frameworkProviders.GetProductRepository().AddRow(new Product()
                    {
                        ProductName = name,
                        Price = (decimal)21.36,
                        ProductDescription = ProductDescription,
                        Supplier = "None"
                    }));
                    sw.Stop();
                    progress.Report((double)i / samplesQuantity);
                }
            }

            return addedIds;
        }

        private void RemoveRecordsSilent(IEnumerable<int> addedIds)
        {
            int i = 0;
            using (var progress = new ProgressBar())
            {
                foreach (var addedId in addedIds)
                {
                    _frameworkProviders.GetProductRepository().DeleteRow(addedId);
                    progress.Report((double)i / addedIds.Count());
                    i++;
                }
            }
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
