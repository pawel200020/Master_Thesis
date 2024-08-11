using System;
using System.Collections.Generic;

namespace EntityFrameworkSqlite.Models;

public partial class Store
{
    public int StoreId { get; set; }

    public string Address { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
