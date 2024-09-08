using System.Data;
using System.Data.SqlClient;
using DatabaseFramework.Models;
using DatabaseFramework.Utilities;

namespace DatabaseFramework.Repos
{
    public class EmployeeRepository(string connectionString) : AbstractRepository<Employee>(connectionString)
    {
        private readonly string _connectionString = connectionString;

        public override Employee? GetById(int id)
            => GetById(id, ParseToObject, GenerateQuery);

        public override IEnumerable<Employee> FilterFields(WhereParameter param)
            => FilterFields(param, ParseToObject, GenerateQuery);

        public override int AddRow(Employee entity)
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
                    if (entity.Position is not null && entity.Position.PositionId < 0)
                    {
                        commandText =
                            @$"INSERT INTO {nameof(Position)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Position>())})
Values (@Name, @CarParkingPlace, @PositionBonus) SELECT SCOPE_IDENTITY()";
                        command.Parameters.Add("@Name", SqlDbType.VarChar);
                        command.Parameters.Add("@CarParkingPlace", SqlDbType.VarChar);
                        command.Parameters.Add("@PositionBonus", SqlDbType.Decimal);

                        command.Parameters["@Name"].Value = entity.Position.PositionName;
                        command.Parameters["@CarParkingPlace"].Value = entity.Position.CarParkingPlace is null
                            ? DBNull.Value
                            : entity.Position.CarParkingPlace;
                        command.Parameters["@PositionBonus"].Value = entity.Position.PositionBonus is null
                            ? DBNull.Value
                            : entity.Position.PositionBonus;
                        command.CommandText = commandText;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                entity.PositionId = (int) reader.GetDecimal(0);
                            }
                        }
                    }

                    commandText =
                        @$"INSERT INTO {nameof(Employee)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Employee>())})
Values (@Name, @LastName, @PhoneNumber, @Address, @PositionId) SELECT SCOPE_IDENTITY()";
                    command.Parameters.Add("@Name", SqlDbType.VarChar);
                    command.Parameters.Add("@LastName", SqlDbType.VarChar);
                    command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                    command.Parameters.Add("@Address", SqlDbType.VarChar);
                    command.Parameters.Add("@PositionId", SqlDbType.Int);

                    command.Parameters["@Name"].Value = entity.FirstName;
                    command.Parameters["@LastName"].Value = entity.LastName;
                    command.Parameters["@Address"].Value = entity.Address;
                    command.Parameters["@PhoneNumber"].Value = entity.PhoneNumber is null
                        ? DBNull.Value
                        : entity.PhoneNumber;
                    command.Parameters["@PositionId"].Value = entity.PositionId is null
                        ? DBNull.Value
                        : entity.PositionId;

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

        public override bool EditRow(Employee entity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                string commandText = @$"UPDATE {nameof(Employee)}s SET 
{nameof(Employee.FirstName)} = @FirstName, 
{nameof(Employee.LastName)} = @LastName,
{nameof(Employee.PhoneNumber)} = @PhoneNumber, 
{nameof(Employee.Address)} = @Address,
{nameof(Employee.PositionId)} = @PositionId
WHERE {IdField} = @ID
";

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters.Add("@FirstName", SqlDbType.VarChar);
                command.Parameters.Add("@LastName", SqlDbType.VarChar);
                command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                command.Parameters.Add("@Address", SqlDbType.VarChar);
                command.Parameters.Add("@PositionId", SqlDbType.Int);
                command.Parameters["@ID"].Value = entity.EmployeeId;
                command.Parameters["@FirstName"].Value = entity.FirstName;
                command.Parameters["@LastName"].Value = entity.LastName;
                command.Parameters["@PositionId"].Value = entity.PositionId is null ? DBNull.Value : entity.PositionId;
                command.Parameters["@PhoneNumber"].Value = entity.PhoneNumber;
                command.Parameters["@Address"].Value = entity.Address;

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public override IEnumerable<Employee> RunCustomQuery(SqlCommand command) 
            => RunCustomQuery(command, ParseBaseObject);

        private Employee ParseBaseObject(SqlDataReader reader)
        {
            var employee = new Employee()
            {
                EmployeeId = GetInt(reader, $"{nameof(Employee)}_{nameof(Employee.EmployeeId)}"),
                FirstName = reader.GetString($"{nameof(Employee)}_{nameof(Employee.FirstName)}"),
                LastName = reader.GetString($"{nameof(Employee)}_{nameof(Employee.LastName)}"),
                PhoneNumber = SafeGetString(reader, $"{nameof(Employee)}_{nameof(Employee.PhoneNumber)}"),
                Address = reader.GetString($"{nameof(Employee)}_{nameof(Employee.Address)}"),
                PositionId = SafeGetInt(reader, $"{nameof(Employee)}_{nameof(Employee.PositionId)}")
            };
            return employee;
        }

        private Employee ParseToObject(SqlDataReader reader)
        {
            var employee = new Employee()
            {
                EmployeeId = GetInt(reader, $"{nameof(Employee)}_{nameof(Employee.EmployeeId)}"),
                FirstName = reader.GetString($"{nameof(Employee)}_{nameof(Employee.FirstName)}"),
                LastName = reader.GetString($"{nameof(Employee)}_{nameof(Employee.LastName)}"),
                PhoneNumber = SafeGetString(reader, $"{nameof(Employee)}_{nameof(Employee.PhoneNumber)}"),
                Address = reader.GetString($"{nameof(Employee)}_{nameof(Employee.Address)}"),
                PositionId = SafeGetInt(reader, $"{nameof(Employee)}_{nameof(Employee.PositionId)}")
            };
            if (employee.PositionId.HasValue)
            {
                employee.Position = new Position()
                {
                    CarParkingPlace = SafeGetString(reader, $"{nameof(Position)}_{nameof(Position.CarParkingPlace)}"),
                    PositionBonus = SafeGetDecimal(reader, $"{nameof(Position)}_{nameof(Position.PositionBonus)}"),
                    PositionId = employee.PositionId.Value,
                    PositionName = GetString(reader, $"{nameof(Position)}_{nameof(Position.PositionName)}"),
                };
            }
            return employee;
        }

        private string GenerateQuery()
        {
            var positionDbFields = GetDbFieldsWithoutId<Position>(GlobalConstants.PositionPrefix);
            var positionIdField = GetIdField<Position>();

            string commandText = @$"SELECT {string.Join(", ",
            [string.Join(", ",GetDbFields(GlobalConstants.EmployeePrefix)),
                string.Join(", ",positionDbFields)])} 
FROM {nameof(Employee)}s as {GlobalConstants.EmployeePrefix}
FULL JOIN {nameof(Position)}s as {GlobalConstants.PositionPrefix} ON {GlobalConstants.PositionPrefix}.{positionIdField} = {GlobalConstants.EmployeePrefix}.{positionIdField}";

            return commandText;
        }
    }
}
