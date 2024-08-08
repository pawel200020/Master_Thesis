namespace NHibernateModels;

public class Client
{
    public virtual int ClientId { get; set; }

    public virtual string FirstName { get; set; } = null!;

    public virtual string LastName { get; set; } = null!;

    public virtual string? PhoneNumber { get; set; }

    public virtual string Address { get; set; } = null!;

    public virtual string? Country { get; set; }
}
