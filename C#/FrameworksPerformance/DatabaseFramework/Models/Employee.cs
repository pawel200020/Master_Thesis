using DatabaseFramework.CustomAttributes;

namespace DatabaseFramework.Models;

public partial class Employee
{
    [Id]
    [DbField]
    public int EmployeeId { get; set; }

    [DbField]
    public string FirstName { get; set; } = null!;

    [DbField]
    public string LastName { get; set; } = null!;

    [DbField]
    public string? PhoneNumber { get; set; }

    [DbField]
    public string Address { get; set; } = null!;

    [DbField]
    public int? PositionId { get; set; }

    public virtual Position? Position { get; set; }
}
