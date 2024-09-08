using System.Data;
using System.Data.SqlClient;
using DatabaseFramework.Models;
using DatabaseFramework.Utilities;

namespace DatabaseFramework.Repos
{
    public class StoreRepository(string connectionString) : AbstractRepository<Store>(connectionString)
    {
        public override Store? GetById(int id)
            => GetById(id, ParseToObject, GenerateQuery);

        public override IEnumerable<Store> FilterFields(WhereParameter param)
            => FilterFields(param, ParseToObject, GenerateQuery);

        public override int AddRow(Store entity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var commandText =
                      @$"INSERT INTO {nameof(Store)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Store>())})
Values (@Address, @Country) SELECT SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@Address", SqlDbType.VarChar);
                command.Parameters.Add("@Country", SqlDbType.VarChar);

                command.Parameters["@Address"].Value = entity.Address;
                command.Parameters["@Country"].Value = entity.Country;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var result = (int)reader.GetDecimal(0);
                        connection.Close();
                        return result;
                    }
                }

            }

            return -1;
        }

        public override bool EditRow(Store entity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                string commandText = @$"UPDATE {nameof(Position)}s SET 
{nameof(Store.Address)} = @Address, 
{nameof(Store.Country)} = @Country,
WHERE {IdField} = @ID
";

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@Address", SqlDbType.VarChar);
                command.Parameters.Add("@Country", SqlDbType.VarChar);
                command.Parameters["@ID"].Value = entity.StoreId;

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public override IEnumerable<Store> RunCustomQuery(SqlCommand command)
            => RunCustomQuery(command, ParseToObject);

        private string GenerateQuery()
        {
            string commandText = @$"SELECT {string.Join(", ", GetDbFields(GlobalConstants.StoresPrefix))} 
FROM {nameof(Store)}s as {GlobalConstants.StoresPrefix}";
            return commandText;
        }

        private Store ParseToObject(SqlDataReader reader) =>
            new()
            {
                StoreId = reader.GetInt32($"{nameof(Store)}_{nameof(Store.StoreId)}"),
                Address = reader.GetString($"{nameof(Store)}_{nameof(Store.Address)}"),
                Country = reader.GetString($"{nameof(Store)}_{nameof(Store.Country)}")
            };
    }
}
