using DatabaseFramework.Repos;

namespace DatabaseFramework.Abstraction
{
    internal interface IEntitiesManager
    {
        ProductRepository GetProductRepository();
        OrdersRepository GetOrdersRepository();
        EmployeeRepository GetEmployeeRepository();
        PositionRepository GetPositionRepository();
        ClientRepository GetClientRepository();
        StoreRepository GetStoreRepository();

    }
}
