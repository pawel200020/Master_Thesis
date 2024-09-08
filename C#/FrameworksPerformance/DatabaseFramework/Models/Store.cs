using DatabaseFramework.CustomAttributes;

namespace DatabaseFramework.Models;

public partial class Store
{
    [Id]
    [DbField]
    public int StoreId { get; set; }

    [DbField]
    public string Address { get; set; } = null!;

    [DbField]
    public string Country { get; set; } = null!;
}
