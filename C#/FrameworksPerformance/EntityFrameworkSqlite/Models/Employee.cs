using System;
using System.Collections.Generic;

namespace EntityFrameworkSqlite.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Adress { get; set; } = null!;

    public int PositionId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Position Position { get; set; } = null!;
}
