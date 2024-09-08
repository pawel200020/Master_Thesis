using DatabaseFramework.Abstraction;
using DatabaseFramework.CustomAttributes;

namespace DatabaseFramework.Models;

public partial class Product
{
    [Id]
    [DbField]
    public int ProductId { get; set; }

    [DbField]
    public string ProductName { get; set; } = null!;

    [DbField]
    public string? ProductDescription { get; set; }

    [DbField]
    public decimal Price { get; set; }

    [DbField]
    public string Supplier { get; set; } = null!;
}