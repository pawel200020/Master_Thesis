namespace NHibernateModels;

public class Position
{
    public virtual int PositionId { get; set; }

    public virtual string PositionName { get; set; } = null!;

    public virtual string? CarParkingPlace { get; set; }

    public virtual decimal? PositionBonus { get; set; }
}
