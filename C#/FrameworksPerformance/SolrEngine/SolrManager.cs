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
        private const string url = "http://localhost:8983/solr";
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
            var rows = (int)CountRows(ProductSolrFields[nameof(Product.Id)],
                _solrProducts);
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var idToSearch = _random.Next(rows) + 1;
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = ContentSearch<Product>(idToSearch.ToString(), [nameof(Product.Id)], ProductSolrFields,
                        _productDefaultResultFields,
                        _solrProducts);
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }

            return testResult;
        }

        public TestResult SetOfDataSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));
            var names = ReadData("Files/first_words.txt").Distinct();
            var namesQuantity = names.Count();
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < samplesQuantity; i++)
                {
                    var nameToSearch = names.ElementAt(_random.Next(namesQuantity));
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = ContentSearch<Client>(nameToSearch, [nameof(Client.Name)], ClientSolrFields, _clientDefaultResultFields, _solrClients);
                    sw.Stop();
                    testResult.AddMeasure(sw.Elapsed.TotalMilliseconds);
                    progress.Report((double)i / samplesQuantity);
                }
            }
            return testResult;
        }

        public TestResult SetOfDataWithIsNullSearch(int samplesQuantity)
        {
            var testResult = new TestResult(samplesQuantity, nameof(SingleRecordSearch));
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
            {nameof(Client.Age), "price"},
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

        private IEnumerable<T> ContentSearch<T>(string phrase,
            IEnumerable<string> searchFields,
            Dictionary<string, string> solrFieldsFromNames,
            string[] resultFields,
            ISolrOperations<T> solr)
        {
            var solrFields = searchFields.Select(x => solrFieldsFromNames[x]).ToArray();
            var opt = new QueryOptions();
            opt.Fields = resultFields;
            var extraParams = new Dictionary<string, string>();
            extraParams.Add("defType", "edismax");
            extraParams.Add("wt", "xml");
            var qf = string.Join(" ", solrFields);
            extraParams.Add("qf", qf);
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
