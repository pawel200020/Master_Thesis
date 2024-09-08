using DatabaseFramework.Abstraction;
using DatabaseFramework.CustomAttributes;

namespace DatabaseFramework.Models;

public class Order : IDatabaseEntity
{
    public static string? TableName => "Orders";

    [Id]
    [DbField]
    public int OrderId { get; set; }

    [DbField]
    public int ClientId { get; set; }

    [DbField]
    public int EmployeeId { get; set; }

    [DbField]
    public DateTime? OrderDate { get; set; }

    [DbField]
    public string OrderDetails { get; set; } = null!;

    [DbField]
    public decimal TotalCost { get; set; }

    [DbField]
    public int StoreId { get; set; }

    public Client Client { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public Store Store { get; set; } = null!;
}
