using System;
using System.Collections.Generic;

namespace EntityFrameworkSqlite.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int ClientId { get; set; }

    public int EmployeeId { get; set; }

    public string? OrderDate { get; set; }

    public string OrderDetails { get; set; } = null!;

    public double TotalCost { get; set; }

    public int? StoreId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Store? Store { get; set; }
}
