namespace NHibernateModels;

public class Product
{
    public virtual int ProductId { get; set; }

    public virtual string ProductName { get; set; } = null!;

    public virtual string? ProductDescription { get; set; }

    public virtual decimal Price { get; set; }

    public virtual string Supplier { get; set; } = null!;
}
