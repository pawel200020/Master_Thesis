namespace NHibernateModels;

public class Order
{
    public virtual int OrderId { get; set; }

    public virtual DateTime? OrderDate { get; set; }

    public virtual string OrderDetails { get; set; } = null!;

    public virtual decimal TotalCost { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
