namespace NHibernateModels;

public class Employee
{
    public virtual int EmployeeId { get; set; }

    public virtual string FirstName { get; set; } = null!;

    public virtual string LastName { get; set; } = null!;

    public virtual string? PhoneNumber { get; set; }

    public virtual string Address { get; set; } = null!;

    //public virtual int? PositionId { get; set; }
    public virtual Position? Position { get; set; }
}
