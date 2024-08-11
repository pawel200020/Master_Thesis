namespace NHibernateModels;

public class Store
{
    public virtual int StoreId { get; set; }

    public virtual string Address { get; set; } = null!;

    public virtual string Country { get; set; } = null!;
}
