using DatabaseFramework.CustomAttributes;

namespace DatabaseFramework.Models;

public partial class Position
{
    [Id]
    [DbField]
    public int PositionId { get; set; }

    [DbField]
    public string PositionName { get; set; } = null!;

    [DbField]
    public string? CarParkingPlace { get; set; }

    [DbField]
    public decimal? PositionBonus { get; set; }
}
