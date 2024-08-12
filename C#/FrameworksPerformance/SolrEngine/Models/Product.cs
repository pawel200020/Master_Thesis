using SolrNet.Attributes;

namespace SolrEngine.Models;

internal class Product
{

    [SolrUniqueKey("id")]
    public string Id { get; set; }
    [SolrField("name")]
    public string Name { get; set; }
    public int Salary { get; set; }
    [SolrField("description")]
    public string Description { get; set; }
    [SolrField("price")] 
    public double Price { get; set; }
    [SolrField("category")]
    public double Category { get; set; }
}