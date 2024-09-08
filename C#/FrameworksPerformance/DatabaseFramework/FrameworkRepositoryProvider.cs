using DatabaseFramework.Abstraction;
using DatabaseFramework.Repos;

namespace DatabaseFramework
{
    public class FrameworkRepositoryProvider(string connectionString) : IEntitiesManager
    {
        private OrdersRepository? _ordersRepository;
        private EmployeeRepository? _employeeRepository;
        private PositionRepository? _positionRepository;
        private ProductRepository? _productsRepository;
        private ClientRepository? _clientsRepository;
        private StoreRepository? _storesRepository;
        
        public OrdersRepository GetOrdersRepository() 
            => _ordersRepository ??= new OrdersRepository(connectionString);

        public EmployeeRepository GetEmployeeRepository()
            => _employeeRepository ??= new EmployeeRepository(connectionString);

        public PositionRepository GetPositionRepository()
            => _positionRepository ??= new PositionRepository(connectionString);

        public ProductRepository GetProductRepository()
            => _productsRepository ??= new ProductRepository(connectionString);

        public ClientRepository GetClientRepository()
            => _clientsRepository ??= new ClientRepository(connectionString);

        public StoreRepository GetStoreRepository()
            => _storesRepository ??= new StoreRepository(connectionString);
    }
}
