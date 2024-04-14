using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class Position
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public string? CarParkingPlace { get; set; }

    public decimal? PositionBonus { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
