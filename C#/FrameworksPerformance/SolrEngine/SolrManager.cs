using Common;
using Common.Managers;
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

        public SolrManager()
        {
            InitIfNeeded();
            _solrProducts = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
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
            throw new NotImplementedException();
        }

        public TestResult SetOfDataSearch(int samplesQuantity)
        {
            throw new NotImplementedException();
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


        private static readonly Dictionary<string, string> ProductSolrFields = new()
        {
            {nameof(Product.Id), "id"},
            {nameof(Product.Name), "name"},
            {nameof(Product.Category), "category"},
            {nameof(Product.Price),"price"},
            {nameof(Product.Description),"description"}

        };

        private readonly string[] _defaultResultFields = new[]
        {
            ProductSolrFields[nameof(Product.Id)],
            ProductSolrFields[nameof(Product.Name)],
            ProductSolrFields[nameof(Product.Description)],
            ProductSolrFields[nameof(Product.Category)],
            ProductSolrFields[nameof(Product.Price)],
            "score"
        };

       

        private IEnumerable<Product> ContentSearch(string phrase, IEnumerable<string> fields)
        {
            var solrFields = fields.Select(x => ProductSolrFields[x]).ToArray();
            var opt = new QueryOptions();
            opt.Fields = _defaultResultFields;
            var extraParams = new Dictionary<string, string>();
            extraParams.Add("defType", "edismax");
            extraParams.Add("wt", "xml");
            var qf = string.Join(" ", solrFields);
            extraParams.Add("qf", qf);
            opt.ExtraParams = extraParams;
            return _solrProducts.Query(phrase, opt);
        }
    }
}
