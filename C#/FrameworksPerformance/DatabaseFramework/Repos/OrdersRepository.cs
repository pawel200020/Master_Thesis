using DatabaseFramework.Models;
using DatabaseFramework.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseFramework.Repos
{
    public class OrdersRepository(string connectionString) : AbstractRepository<Order>(connectionString)
    {
        private readonly string _connectionString = connectionString;

        public override Order? GetById(int id)
            => GetById(id, ParseToObject, GenerateQuery);

        public override IEnumerable<Order> FilterFields(WhereParameter param)
            => FilterFields(param, ParseToObject, GenerateQuery);

        public override int AddRow(Order entity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                var transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;
                string commandText = string.Empty;

                try
                {
                    if (entity.Employee.Position is not null && entity.Employee.Position.PositionId < 0)
                    {
                        commandText =
                            @$"INSERT INTO {nameof(Position)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Position>())})
Values (@Name, @CarParkingPlace, @PositionBonus) SELECT SCOPE_IDENTITY()";
                        command.Parameters.Add("@Name", SqlDbType.VarChar);
                        command.Parameters.Add("@CarParkingPlace", SqlDbType.VarChar);
                        command.Parameters.Add("@PositionBonus", SqlDbType.Decimal);

                        command.Parameters["@Name"].Value = entity.Employee.Position.PositionName;
                        command.Parameters["@CarParkingPlace"].Value = entity.Employee.Position.CarParkingPlace is null
                            ? DBNull.Value
                            : entity.Employee.Position.CarParkingPlace;
                        command.Parameters["@PositionBonus"].Value = entity.Employee.Position.PositionBonus is null
                            ? DBNull.Value
                            : entity.Employee.Position.PositionBonus;
                        command.CommandText = commandText;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                entity.Employee.PositionId = (int) reader.GetDecimal(0);
                                entity.Employee.Position.PositionId = entity.Employee.PositionId.Value;
                            }
                        }
                    }

                    if (entity.Employee.EmployeeId < 0)
                    {
                        commandText =
                            @$"INSERT INTO {nameof(Employee)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Employee>())})
Values (@Name, @LastName, @PhoneNumber, @Address, @PositionId) SELECT SCOPE_IDENTITY()";
                        command.Parameters.Add("@Name", SqlDbType.VarChar);
                        command.Parameters.Add("@LastName", SqlDbType.VarChar);
                        command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                        command.Parameters.Add("@Address", SqlDbType.VarChar);
                        command.Parameters.Add("@PositionId", SqlDbType.Int);

                        command.Parameters["@Name"].Value = entity.Employee.FirstName;
                        command.Parameters["@LastName"].Value = entity.Employee.LastName;
                        command.Parameters["@Address"].Value = entity.Employee.Address;
                        command.Parameters["@PhoneNumber"].Value = entity.Employee.PhoneNumber is null
                            ? DBNull.Value
                            : entity.Employee.PhoneNumber;
                        command.Parameters["@PositionId"].Value = entity.Employee.PositionId is null
                            ? DBNull.Value
                            : entity.Employee.PositionId;

                        command.CommandText = commandText;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                entity.Employee.EmployeeId = (int) reader.GetDecimal(0);
                            }
                        }
                    }

                    if (entity.Client.ClientId < 0)
                    {
                        commandText =
                            @$"INSERT INTO {nameof(Client)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Client>())})
Values (@Name, @LastName, @PhoneNumber, @Address, @Country) SELECT SCOPE_IDENTITY()";
                        command.Parameters.Add("@Name", SqlDbType.VarChar);
                        command.Parameters.Add("@LastName", SqlDbType.VarChar);
                        command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                        command.Parameters.Add("@Address", SqlDbType.VarChar);
                        command.Parameters.Add("@Country", SqlDbType.VarChar);

                        command.Parameters["@Name"].Value = entity.Client.FirstName;
                        command.Parameters["@LastName"].Value = entity.Client.LastName;
                        command.Parameters["@Address"].Value = entity.Client.Address;
                        command.Parameters["@PhoneNumber"].Value = entity.Client.PhoneNumber is null
                            ? DBNull.Value
                            : entity.Client.PhoneNumber;
                        command.Parameters["@Country"].Value = entity.Client.Country;
                        command.CommandText = commandText;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                entity.Client.ClientId = (int) reader.GetDecimal(0);
                        }
                    }

                    if (entity.Store.StoreId < 0)
                    {
                        commandText =
                            @$"INSERT INTO {nameof(Store)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Store>())})
Values (@Address, @Country) SELECT SCOPE_IDENTITY()";
                        command.Parameters.Add("@Address", SqlDbType.VarChar);
                        command.Parameters.Add("@Country", SqlDbType.VarChar);

                        command.Parameters["@Address"].Value = entity.Store.Address;
                        command.Parameters["@Country"].Value = entity.Store.Country;
                        command.CommandText = commandText;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                entity.Store.StoreId = (int) reader.GetDecimal(0);
                        }
                    }

                    commandText =
                        @$"INSERT INTO {nameof(Order)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Order>())})
Values (@ClientId, @EmployeeId, @OrderDate, @OrderDetails, @TotalCost, @StoreId) SELECT SCOPE_IDENTITY()";
                    command.Parameters.Add("@ClientId", SqlDbType.Int);
                    command.Parameters.Add("@EmployeeId", SqlDbType.Int);
                    command.Parameters.Add("@OrderDate", SqlDbType.DateTime);
                    command.Parameters.Add("@OrderDetails", SqlDbType.VarChar);
                    command.Parameters.Add("@TotalCost", SqlDbType.Decimal);
                    command.Parameters.Add("@StoreId", SqlDbType.Int);

                    command.Parameters["@ClientId"].Value = entity.Client.ClientId;
                    command.Parameters["@EmployeeId"].Value = entity.Employee.EmployeeId;
                    command.Parameters["@OrderDate"].Value = entity.OrderDate;
                    command.Parameters["@OrderDetails"].Value = entity.OrderDetails;
                    command.Parameters["@OrderDetails"].Value = entity.OrderDetails;
                    command.Parameters["@TotalCost"].Value = entity.TotalCost;
                    command.Parameters["@StoreId"].Value = entity.Store.StoreId;
                    command.CommandText = commandText;
                    int result = -1;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            result = (int) reader.GetDecimal(0);
                    }
                    transaction.Commit();
                    return result;
                }
                catch (Exception e)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }
            return -1;
        }

        public override bool EditRow(Order entity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                string commandText = @$"UPDATE {nameof(Order)}s SET 
{nameof(Order.ClientId)} = @ClientId, 
{nameof(Order.EmployeeId)} = @EmployeeId,
{nameof(Order.OrderDate)} = @OrderDate, 
{nameof(Order.OrderDetails)} = @OrderDetails,
{nameof(Order.TotalCost)} = @TotalCost, 
{nameof(Order.StoreId)} = @StoreId
WHERE {IdField} = @ID
";

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters.Add("@ClientId", SqlDbType.Int);
                command.Parameters.Add("@EmployeeId", SqlDbType.Int);
                command.Parameters.Add("@OrderDate", SqlDbType.DateTime);
                command.Parameters.Add("@OrderDetails", SqlDbType.VarChar);
                command.Parameters.Add("@TotalCost", SqlDbType.Decimal);
                command.Parameters.Add("@StoreId", SqlDbType.Int);
                command.Parameters["@ID"].Value = entity.OrderId;
                command.Parameters["@ClientId"].Value = entity.ClientId;
                command.Parameters["@EmployeeId"].Value = entity.EmployeeId;
                command.Parameters["@OrderDate"].Value = entity.OrderDate is null ? DBNull.Value : entity.OrderDate;
                command.Parameters["@OrderDetails"].Value = entity.OrderDetails;
                command.Parameters["@TotalCost"].Value = entity.TotalCost;
                command.Parameters["@StoreId"].Value = entity.StoreId;

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public override IEnumerable<Order> RunCustomQuery(SqlCommand command) 
            => RunCustomQuery(command, ParseBaseObject);

        private string GenerateQuery()
        {
         
            var clientDbFields = GetDbFieldsWithoutId<Client>(GlobalConstants.ClientPrefix);
            var clientIdField = GetIdField<Client>();

            var employeeDbFields = GetDbFieldsWithoutId<Employee>(GlobalConstants.EmployeePrefix);
            var employeeIdField = GetIdField<Employee>();

            var positionDbFields = GetDbFieldsWithoutId<Position>(GlobalConstants.PositionPrefix);
            var positionIdField = GetIdField<Position>();
         
            var storeDbFields = GetDbFieldsWithoutId<Store>(GlobalConstants.StoresPrefix);
            var storeIdField = GetIdField<Store>();

            string commandText = @$"SELECT {string.Join(", ",
            [string.Join(", ",GetDbFields(GlobalConstants.OrdersPrefix)),
                string.Join(", ", clientDbFields),
                string.Join(", ",employeeDbFields),
                string.Join(", ",positionDbFields),
                string.Join(", ",storeDbFields)])} 
FROM {nameof(Order)}s as {GlobalConstants.OrdersPrefix}
INNER JOIN {nameof(Client)}s as {GlobalConstants.ClientPrefix} ON {GlobalConstants.ClientPrefix}.{clientIdField} = {GlobalConstants.OrdersPrefix}.{clientIdField}
INNER JOIN {nameof(Employee)}s as {GlobalConstants.EmployeePrefix} ON {GlobalConstants.EmployeePrefix}.{employeeIdField} = {GlobalConstants.OrdersPrefix}.{employeeIdField}
FULL JOIN {nameof(Position)}s as {GlobalConstants.PositionPrefix} ON {GlobalConstants.PositionPrefix}.{positionIdField} = {GlobalConstants.EmployeePrefix}.{positionIdField}
INNER JOIN {nameof(Store)}s as {GlobalConstants.StoresPrefix} ON {GlobalConstants.StoresPrefix}.{storeIdField} = {GlobalConstants.OrdersPrefix}.{storeIdField}";

            return commandText;
        }

        private Order ParseBaseObject(SqlDataReader reader)
        {
            return new Order()
            {
                OrderId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.OrderId)}"),
                ClientId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.ClientId)}"),
                EmployeeId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.EmployeeId)}"),
                OrderDate = SafeGetDate(reader, $"{nameof(Order)}_{nameof(Order.OrderDate)}"),
                OrderDetails = GetString(reader, $"{nameof(Order)}_{nameof(Order.OrderDetails)}"),
                TotalCost = GetDecimal(reader, $"{nameof(Order)}_{nameof(Order.TotalCost)}"),
                StoreId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.StoreId)}")
            };
        }

        private Order ParseToObject(SqlDataReader reader)
        {
            var order = new Order()
            {
                OrderId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.OrderId)}"),
                ClientId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.ClientId)}"),
                EmployeeId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.EmployeeId)}"),
                OrderDate = SafeGetDate(reader, $"{nameof(Order)}_{nameof(Order.OrderDate)}"),
                OrderDetails = GetString(reader, $"{nameof(Order)}_{nameof(Order.OrderDetails)}"),
                TotalCost = GetDecimal(reader, $"{nameof(Order)}_{nameof(Order.TotalCost)}"),
                StoreId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.StoreId)}"),
                Client = new Client()
                {
                    ClientId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.ClientId)}"),
                    FirstName = reader.GetString($"{nameof(Client)}_{nameof(Client.FirstName)}"),
                    LastName = reader.GetString($"{nameof(Client)}_{nameof(Client.LastName)}"),
                    PhoneNumber = SafeGetString(reader, $"{nameof(Client)}_{nameof(Client.PhoneNumber)}"),
                    Address = reader.GetString($"{nameof(Client)}_{nameof(Client.Address)}"),
                    Country = SafeGetString(reader, $"{nameof(Client)}_{nameof(Client.Country)}"),
                },
                Employee = new Employee()
                {
                    EmployeeId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.EmployeeId)}"),
                    FirstName = reader.GetString($"{nameof(Employee)}_{nameof(Employee.FirstName)}"),
                    LastName = reader.GetString($"{nameof(Employee)}_{nameof(Employee.LastName)}"),
                    PhoneNumber = SafeGetString(reader,$"{nameof(Employee)}_{nameof(Employee.PhoneNumber)}"),
                    Address = reader.GetString($"{nameof(Employee)}_{nameof(Employee.Address)}"),
                    PositionId = SafeGetInt(reader, $"{nameof(Employee)}_{nameof(Employee.PositionId)}")
                },
                Store = new Store()
                {
                    StoreId = GetInt(reader, $"{nameof(Order)}_{nameof(Order.EmployeeId)}"),
                    Address = reader.GetString($"{nameof(Store)}_{nameof(Store.Address)}"),
                    Country = reader.GetString($"{nameof(Store)}_{nameof(Store.Country)}"),
                }
            };
            if (order.Employee.PositionId.HasValue)
            {
                order.Employee.Position = new Position()
                {
                    CarParkingPlace = SafeGetString(reader, $"{nameof(Position)}_{nameof(Position.CarParkingPlace)}"),
                    PositionBonus = SafeGetDecimal(reader, $"{nameof(Position)}_{nameof(Position.PositionBonus)}"),
                    PositionId = order.Employee.PositionId.Value,
                    PositionName = GetString(reader, $"{nameof(Position)}_{nameof(Position.PositionName)}"),
                };
            }

            return order;
        }
    }
}
