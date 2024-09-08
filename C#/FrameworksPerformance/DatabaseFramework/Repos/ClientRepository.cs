using DatabaseFramework.Models;
using DatabaseFramework.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseFramework.Repos
{
    public class ClientRepository(string connectionString) : AbstractRepository<Client>(connectionString)
    {
        public override Client? GetById(int id)
            => GetById(id, ParseToObject, GenerateQuery);

        public override IEnumerable<Client> FilterFields(WhereParameter param)
            => FilterFields(param, ParseToObject, GenerateQuery);

        public override int AddRow(Client entity)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandText = string.Empty;

                commandText =
                    @$"INSERT INTO {nameof(Client)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Client>())})
Values (@Name, @LastName, @PhoneNumber, @Address, @PositionId) SELECT SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters.Add("@LastName", SqlDbType.VarChar);
                command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                command.Parameters.Add("@Address", SqlDbType.VarChar);
                command.Parameters.Add("@Country", SqlDbType.VarChar);

                command.Parameters["@Name"].Value = entity.FirstName;
                command.Parameters["@LastName"].Value = entity.LastName;
                command.Parameters["@Address"].Value = entity.Address;
                command.Parameters["@PhoneNumber"].Value = entity.PhoneNumber is null
                    ? DBNull.Value
                    : entity.PhoneNumber;
                command.Parameters["@Country"].Value = entity.Country is null
                    ? DBNull.Value
                    : entity.Country;

                command.CommandText = commandText;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var result = (int)reader.GetDecimal(0);
                        connection.Close();
                        return result;
                    }
                    connection.Close();
                }
            }
            return -1;
        }

    public override bool EditRow(Client entity)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            if (IdField is null)
                throw new InvalidOperationException("Id field not detected!");

            string commandText = @$"UPDATE {nameof(Client)}s SET 
{nameof(Client.FirstName)} = @FirstName, 
{nameof(Client.LastName)} = @LastName,
{nameof(Client.PhoneNumber)} = @PhoneNumber, 
{nameof(Client.Address)} = @Address,
{nameof(Client.Country)} = @Country
WHERE {IdField} = @ID
";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Add("@ID", SqlDbType.Int);
            command.Parameters.Add("@FirstName", SqlDbType.VarChar);
            command.Parameters.Add("@LastName", SqlDbType.VarChar);
            command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
            command.Parameters.Add("@Address", SqlDbType.VarChar);
            command.Parameters.Add("@PositionId", SqlDbType.Int);
            command.Parameters["@ID"].Value = entity.ClientId;
            command.Parameters["@FirstName"].Value = entity.FirstName;
            command.Parameters["@LastName"].Value = entity.LastName;
            command.Parameters["@Country"].Value = entity.Country is null ? DBNull.Value : entity.Country;
            command.Parameters["@PhoneNumber"].Value = entity.PhoneNumber;
            command.Parameters["@Address"].Value = entity.Address;

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }

    public override IEnumerable<Client> RunCustomQuery(SqlCommand command)
        => RunCustomQuery(command, ParseToObject);

    private string GenerateQuery()
    {
        string commandText = @$"SELECT {string.Join(", ", GetDbFields(GlobalConstants.ClientPrefix))} 
FROM {nameof(Client)}s as {GlobalConstants.ClientPrefix}";
        return commandText;
    }

    private Client ParseToObject(SqlDataReader reader) =>
        new()
        {
            ClientId = GetInt(reader, $"{nameof(Client)}_{nameof(Client.ClientId)}"),
            FirstName = reader.GetString($"{nameof(Client)}_{nameof(Client.FirstName)}"),
            LastName = reader.GetString($"{nameof(Client)}_{nameof(Client.LastName)}"),
            PhoneNumber = SafeGetString(reader, $"{nameof(Client)}_{nameof(Client.PhoneNumber)}"),
            Address = reader.GetString($"{nameof(Client)}_{nameof(Client.Address)}"),
            Country = SafeGetString(reader, $"{nameof(Client)}_{nameof(Client.Country)}")
        };
}
}
