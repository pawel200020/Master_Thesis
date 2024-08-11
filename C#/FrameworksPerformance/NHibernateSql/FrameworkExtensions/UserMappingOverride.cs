using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernateModels;

namespace NHibernateSql.FrameworkExtensions
{
    public class UserMappingOverride : 
        IAutoMappingOverride<Client>, 
        IAutoMappingOverride<Position>, 
        IAutoMappingOverride<Store>,
        IAutoMappingOverride<Employee>,
        IAutoMappingOverride<Product>,
        IAutoMappingOverride<Order>
    {
        private const string Id = "Id";
        public void Override(AutoMapping<Client> mapping)
        {
            mapping.Id(u => u.ClientId);
            mapping.Table(TableNames.Clients);
        }

        public void Override(AutoMapping<Position> mapping)
        {
            mapping.Id(u => u.PositionId);
            mapping.Table(TableNames.Positions);
        }
        public void Override(AutoMapping<Store> mapping)
        {

            mapping.Id(u => u.StoreId);
            mapping.Table(TableNames.Stores);
        }

        public void Override(AutoMapping<Employee> mapping)
        {
            mapping.Id(u => u.EmployeeId);
            mapping.Table(TableNames.Employees);
            mapping.References(x => x.Position).Column($"{nameof(Employee.Position)}{Id}").ForeignKey($"{nameof(Employee.Position)}{Id}");
        }

        public void Override(AutoMapping<Order> mapping)
        {
            mapping.Id(u => u.OrderId);
            mapping.Table(TableNames.Orders);
            mapping.References(x => x.Store).Column($"{nameof(Order.Store)}{Id}").ForeignKey($"{nameof(Order.Store)}{Id}");
            mapping.References(x => x.Client).Column($"{nameof(Order.Client)}{Id}").ForeignKey($"{nameof(Order.Client)}{Id}");
            mapping.References(x => x.Employee).Column($"{nameof(Order.Employee)}{Id}").ForeignKey($"{nameof(Order.Employee)}{Id}");
        }

        public void Override(AutoMapping<Product> mapping)
        {
            mapping.Id(u => u.ProductId);
            mapping.Table(TableNames.Products);
        }
    }
}
