using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public decimal Price { get; set; }

    public string Supplier { get; set; } = null!;
}
