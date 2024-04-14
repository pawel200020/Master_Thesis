using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class Store
{
    public int StoreId { get; set; }

    public string Address { get; set; } = null!;

    public string Country { get; set; } = null!;
}
