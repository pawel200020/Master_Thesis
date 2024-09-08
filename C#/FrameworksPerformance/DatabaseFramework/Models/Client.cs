using DatabaseFramework.CustomAttributes;

namespace DatabaseFramework.Models;

public class Client
{
    [Id]
    [DbField]
    public int ClientId { get; set; }

    [DbField]
    public string FirstName { get; set; } = null!;

    [DbField]
    public string LastName { get; set; } = null!;

    [DbField]
    public string? PhoneNumber { get; set; }

    [DbField]
    public string Address { get; set; } = null!;

    [DbField]
    public string? Country { get; set; }
}
