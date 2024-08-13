using SolrNet.Attributes;

namespace SolrEngine.Models;

internal class Client
{
    [SolrUniqueKey("id")]
    public int Id { get; set; }
    [SolrField("name")]
    public string Name { get; set; }
    [SolrField("last_name")]
    public string LastName { get; set; }
    [SolrField("age")]
    public int Age { get; set; }
    [SolrField("salary")]
    public double Salary { get; set; }
    [SolrField("description")]
    public string Description { get; set; }
    [SolrField("favourite_product")]
    public int FavouriteProduct { get; set; }
    [SolrField("recent_bought_products")]
    public string[] RecentBought { get; set; }
    [SolrField("birth_date")]
    public DateTime BirthDate { get; set; }
    [SolrField("has_children_b")]
    public bool? HasChildren { get; set; }
    [SolrField("how_many_cats_i")]
    public int? HowManyCats { get; set; }
    [SolrField("has_partner_b")]
    public bool? HasPartner { get; set; }
    [SolrField("personal_data")]
    public string[] PersonalData { get; set; }
}