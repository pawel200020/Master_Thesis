using SolrNet.Attributes;

namespace SolrEngine.Models;

internal class Product
{

    [SolrUniqueKey("id")]
    public int Id { get; set; }
    [SolrField("name")]
    public string Name { get; set; }
    [SolrField("description")]
    public string Description { get; set; }
    [SolrField("price")] 
    public double Price { get; set; }
    [SolrField("category")]
    public string Category { get; set; }
}