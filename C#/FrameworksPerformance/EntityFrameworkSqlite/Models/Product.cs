using System;
using System.Collections.Generic;

namespace EntityFrameworkSqlite.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public double Price { get; set; }

    public string Supplier { get; set; } = null!;
}
