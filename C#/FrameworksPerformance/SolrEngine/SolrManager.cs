using System.Configuration;
using System.Diagnostics;
using Common;
using Common.Managers;
using Common.Utilites;
using CommonServiceLocator;
using Flurl;
using SolrEngine.Models;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrEngine
{
    public class SolrManager : IFrameworkManager
    {
        private bool _initialized;
        private static string url = ConfigurationManager.AppSettings.Get("SolrUrl")!;
        private readonly ISolrOperations<Product> _solrProducts;
        private readonly ISolrOperations<Client> _solrClients;
        private readonly Random _random = new();

        public SolrManager()
        {
            InitIfNeeded();
            _solrProducts = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            _solrClients = ServiceLocator.Current.GetInstance<ISolrOperations<Client>>();
        }

        private void InitIfNeeded()
        {
            if (!_initialized)
            {
                var connectionToProducts = new SolrConnection(Url.Combine(url, Enum.GetName(SolrCoresEnum.Products)));
                var connectionToClients = new SolrConnection(Url.Combine(url, Enum.GetName(SolrCoresEnum.Clients)));
                Startup.Init<Product>(connectionToProducts);
                Startup.Init<Client>(connectionToClients);
                _initialized = true;
            }
        }

        public TestResult SingleRecordSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));
            var rows = 1000000-1;
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var idToSearch = _random.Next(rows) + 1;
                    var sw = new Stopwatch();
                    string id = idToSearch.ToString();
                    sw.Start();
                    var result = ContentSearch<Product>(id, [nameof(Product.Id)], ProductSolrFields,
                        _productDefaultResultFields,
                        _solrProducts);
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }

            return testResult;
        }
        //public TestResult SingleRecordSearch(int samplesQuantity)
        //{
        //    var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));
        //    var rows = 1000100;
        //    using (var progress = new ProgressBar())
        //    {
        //        for (int i = 0; i < samplesQuantity; i++)
        //        {
        //            var idToSearch = _random.Next(rows) + 1;
        //            var sw = new Stopwatch();
        //            string id = idToSearch.ToString();
        //            sw.Start();
        //            var result = ContentSearch<Product>(id, [nameof(Product.Id)], ProductSolrFields,
        //                _productDefaultResultFields,
        //                _solrProducts);
        //            sw.Stop();
        //            testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
        //            progress.Report((double)i / samplesQuantity);
        //        }
        //    }

        //    return testResult;
        //}

        public TestResult SetOfDataSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SetOfDataSearch));
            var names = ReadData("Files/first_words.txt").Distinct();
            var namesQuantity = names.Count();
            using (var progress = new ProgressBar())
            {
                
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var nameToSearch = "Eco-Friendly*";
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = ContentSearch<Product>("added by test", [nameof(Product.Description)], ProductSolrFields, _productDefaultResultFields, _solrProducts).ToArray();
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
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
                    var field = i % 3 == 0
                        ? "has_children_b"
                        : i % 3 == 1
                            ? "how_many_cats_i"
                            : "has_partner_b";
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = SearchBareQuery<Client>($"NOT {field}:*", _clientDefaultResultFields, _solrClients);
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }
            return testResult;
        }

        //public TestResult AddRecords(int samplesQuantity)
        //{
        //    var testResult = new TestResult(samplesQuantity, nameof(AddRecords));
        //    var id = (int)CountRows(ProductSolrFields[nameof(Product.Id)], _solrProducts) + 1;
        //    var categories = ReadData("Files/categories.txt");
        //    List<string> idsToRemove = new();
        //    using (var progress = new ProgressBar())
        //    {
        //        for (int i = 0; i < samplesQuantity; i++)
        //        {
        //            var category = categories.ElementAt(_random.Next(344));
        //            var price = (double)_random.Next(9999999) / 100;
        //            var sw = new Stopwatch();
        //            sw.Start();
        //            _solrProducts.Add(new Product()
        //            {
        //                Category = category,
        //                Description = "added by test",
        //                Id = id,
        //                Name = $"Example product {i}",
        //                Price = price,
        //                IdInt = id
        //            });

        //            sw.Stop();
        //            idsToRemove.Add(id.ToString());
        //            id++;
        //            testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
        //            progress.Report((double)i / samplesQuantity);
        //        }
        //    }
        //    _solrProducts.Commit();
        //    //_solrProducts.Delete(idsToRemove);
        //    //_solrProducts.Commit();
        //    return testResult;
        //}

        public TestResult AddRecords(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(AddRecords));
            var id = (int)CountRows(ClientSolrFields[nameof(Client.Id)], _solrClients) + 1;
            var categories = ReadData("Files/categories.txt");
            var productNames = ReadData("Files/first_words.txt");
            List<string> idsToRemove = new();
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var name = productNames.ElementAt(_random.Next(productNames.Count()));
                    var sw = new Stopwatch();
                    sw.Start();
                    _solrClients.Add(new Client()
                    {
                        Name = name,
                        LastName = "Nowak",
                        Salary = 1234,
                        Age = 45,
                        FavouriteProduct = _random.Next(1101099) + 1,
                        BirthDate = DateTime.Now,
                        RecentBought = ["High-Quality Books Laptop"],
                        Description = "added by test",

                        Id = id,
                    });

                    sw.Stop();
                    idsToRemove.Add(id.ToString());
                    id++;
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }
            _solrClients.Commit();
            //_solrProducts.Delete(idsToRemove);
            //_solrProducts.Commit();
            return testResult;
        }

        public TestResult EditRecords(int samplesQuantity)
        {
            var addedIds = AddProducts(samplesQuantity);
            var testResult = new TestResult(samplesQuantity, nameof(EditRecords));
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var firstIndex = int.Parse(addedIds.ElementAt(0));
                    var idToEdit = _random.Next(firstIndex, firstIndex + addedIds.Count());
                    var sw = new Stopwatch();
                    sw.Start();
                    _solrProducts.AtomicUpdate(idToEdit.ToString(),
                        new[]
                        {
                            new AtomicUpdateSpec(ProductSolrFields[nameof(Product.Description)], AtomicUpdateType.Set,
                                $"Edited value set for this by iteration {i}")
                        });
                    _solrProducts.Commit();
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }

            }
            _solrProducts.Delete(addedIds);
            _solrProducts.Commit();
            return testResult;
        }

        public TestResult DeleteRecords(int samplesQuantity)
        {
            var addedIds = AddProducts(samplesQuantity);
            var testResult = new TestResult(samplesQuantity, nameof(DeleteRecords));
            int progressAsInt = 0;
            using (var progress = new ProgressBar())
            { 
                foreach (var id in addedIds)
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    _solrProducts.Delete(id);
                    _solrProducts.Commit();
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)progressAsInt / samplesQuantity);
                    progressAsInt++;
                }

            }
            _solrProducts.Commit();
            return testResult;
        }

        public TestResult SearchTwoRelatedTables(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SearchTwoRelatedTables));

            var idsToSearch = ContentSearch<Client>("*", [nameof(Client.Id)], ClientSolrFields,
                _clientDefaultResultFields,
                _solrClients).Select(x => x.FavouriteProduct).Distinct();
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var idToSearch = idsToSearch.ElementAt(_random.Next(idsToSearch.Count() - 1));
                    string q = $"{{!join from=id_int to=favourite_product fromIndex=Products}}id:{idToSearch}";
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = ExecuteQuery<Client>(q,
                        _clientDefaultResultFields,
                        _solrClients).ToArray();
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }

            return testResult;
        }

        private static readonly Dictionary<string, string> ProductSolrFields = new()
        {
            {nameof(Product.Id), "id"},
            {nameof(Product.Name), "name"},
            {nameof(Product.Category), "category"},
            {nameof(Product.Price), "price"},
            {nameof(Product.Description), "description"}
        };

        private static readonly Dictionary<string, string> ClientSolrFields = new()
        {
            {nameof(Client.Id), "id"},
            {nameof(Client.Name), "name"},
            {nameof(Client.LastName), "category"},
            {nameof(Client.Age), "age"},
            {nameof(Client.Salary), "salary"},
            {nameof(Client.Description), "description"},
            {nameof(Client.FavouriteProduct), "favourite_product"},
            {nameof(Client.RecentBought), "recent_bought_products"},
            {nameof(Client.BirthDate), "birth_date"},
            {nameof(Client.PersonalData), "PersonalData"},
            {nameof(Client.HasChildren), "has_children_b"},
            {nameof(Client.HowManyCats), "how_many_cats_i"},
            {nameof(Client.HasPartner), "has_partner_b"},

        };

        private readonly string[] _productDefaultResultFields = new[]
        {
            ProductSolrFields[nameof(Product.Id)],
            ProductSolrFields[nameof(Product.Name)],
            ProductSolrFields[nameof(Product.Description)],
            ProductSolrFields[nameof(Product.Category)],
            ProductSolrFields[nameof(Product.Price)],
        };

        private readonly string[] _clientDefaultResultFields = new[]
        {
            ClientSolrFields[nameof(Client.Id)],
            ClientSolrFields[nameof(Client.Name)],
            ClientSolrFields[nameof(Client.LastName)],
            ClientSolrFields[nameof(Client.Age)],
            ClientSolrFields[nameof(Client.Salary)],
            ClientSolrFields[nameof(Client.Description)],
            ClientSolrFields[nameof(Client.FavouriteProduct)],
            ClientSolrFields[nameof(Client.RecentBought)],
            ClientSolrFields[nameof(Client.BirthDate)],
            ClientSolrFields[nameof(Client.PersonalData)],
            ClientSolrFields[nameof(Client.HasChildren)],
            ClientSolrFields[nameof(Client.HasPartner)],
            ClientSolrFields[nameof(Client.HowManyCats)],
        };

        private IEnumerable<string> AddProducts(int samplesQuantity)
        {
            var id = (int)CountRows(ProductSolrFields[nameof(Product.Id)], _solrProducts) + 1;
            var categories = ReadData("Files/categories.txt");
            var addedIds = new List<string>();
            List<string> idsToRemove = new();
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var category = categories.ElementAt(_random.Next(344));
                    var price = (double)_random.Next(9999999) / 100;
                   
                    _solrProducts.Add(new Product()
                    {
                        Category = category,
                        Description = "added by test",
                        Id = id,
                        Name = $"Example product {i}",
                        Price = price,
                    });
                    addedIds.Add(id.ToString());
                    id++;
                    progress.Report((double)i / samplesQuantity);
                }
            }
            _solrProducts.Commit();
            return addedIds;
        }

        private IEnumerable<T> ContentSearch<T>(string phrase,
            IEnumerable<string> searchFields,
            Dictionary<string, string> solrFieldsFromNames,
            string[] resultFields,
            ISolrOperations<T> solr)
        {
            var solrFields = searchFields.Select(x => solrFieldsFromNames[x]).ToArray();
            var opt = new QueryOptions();
            opt.Rows = 1000;
            opt.Fields = resultFields;
            var extraParams = new Dictionary<string, string>();
            extraParams.Add("defType", "edismax");
            extraParams.Add("wt", "xml");
            var qf = string.Join(" ", solrFields);
            extraParams.Add("qf", qf);
            opt.ExtraParams = extraParams;
            return solr.Query(phrase, opt);
        }

        private IEnumerable<T> ExecuteQuery<T>(string phrase,
            string[] resultFields,
            ISolrOperations<T> solr)
        {
            var opt = new QueryOptions();
            opt.Fields = resultFields;
            var extraParams = new Dictionary<string, string>();
            extraParams.Add("defType", "lucene");
            extraParams.Add("wt", "xml");
            opt.ExtraParams = extraParams;
            return solr.Query(phrase, opt);
        }

        private IEnumerable<T> SearchBareQuery<T>(string phrase,
            string[] resultFields,
            ISolrOperations<T> solr)
        {
            var opt = new QueryOptions();
            opt.Fields = resultFields;
            var extraParams = new Dictionary<string, string>();
            extraParams.Add("defType", "lucene");
            extraParams.Add("wt", "xml");
            opt.ExtraParams = extraParams;
            return solr.Query(phrase, opt);
        }

        private long CountRows<T>(string solrIdFieldName, ISolrOperations<T> solr)
        {
            var opt = new QueryOptions();
            opt.Fields = [solrIdFieldName];
            opt.Rows = 1;
            opt.StartOrCursor = new StartOrCursor.Start(0);
            var extraParams = new Dictionary<string, string>();
            extraParams.Add("defType", "edismax");
            extraParams.Add("wt", "xml");
            var qf = solrIdFieldName;
            extraParams.Add("qf", qf);
            opt.ExtraParams = extraParams;
            return solr.Query("*", opt).NumFound;
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
