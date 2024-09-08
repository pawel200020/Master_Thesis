using System.Data;
using System.Data.SqlClient;
using DatabaseFramework.Models;
using DatabaseFramework.Utilities;

namespace DatabaseFramework.Repos
{
    public class PositionRepository(string connectionString) : AbstractRepository<Position>(connectionString)
    {
        private readonly string _connectionString = connectionString;

        public override Position? GetById(int id)
            => GetById(id, ParseToObject, GenerateQuery);

        public override IEnumerable<Position> FilterFields(WhereParameter param)
            => FilterFields(param, ParseToObject, GenerateQuery);

        public override int AddRow(Position entity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string commandText = string.Empty;

                commandText =
                    @$"INSERT INTO {nameof(Position)}s ({string.Join(", ", GetDbFieldsWithoutIdAndPrefix<Position>())})
Values (@Name, @CarParkingPlace, @PositionBonus) SELECT SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters.Add("@CarParkingPlace", SqlDbType.VarChar);
                command.Parameters.Add("@PositionBonus", SqlDbType.Decimal);

                command.Parameters["@Name"].Value = entity.PositionName;
                command.Parameters["@CarParkingPlace"].Value = entity.CarParkingPlace is null
                    ? DBNull.Value
                    : entity.CarParkingPlace;
                command.Parameters["@PositionBonus"].Value = entity.PositionBonus is null
                    ? DBNull.Value
                    : entity.PositionBonus;
                command.CommandText = commandText;

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

    public override bool EditRow(Position entity)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            if (IdField is null)
                throw new InvalidOperationException("Id field not detected!");

            string commandText = @$"UPDATE {nameof(Position)}s SET 
{nameof(Position.PositionName)} = @PositionName, 
{nameof(Position.CarParkingPlace)} = @CarParkingPlace,
{nameof(Position.PositionBonus)} = @PositionBonus
WHERE {IdField} = @ID
";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Add("@ID", SqlDbType.Int);
            command.Parameters.Add("@PositionName", SqlDbType.VarChar);
            command.Parameters.Add("@CarParkingPlace", SqlDbType.VarChar);
            command.Parameters.Add("@PositionBonus", SqlDbType.Decimal);
            command.Parameters["@ID"].Value = entity.PositionId;
            command.Parameters["@PositionName"].Value = entity.PositionName;
            command.Parameters["@CarParkingPlace"].Value = entity.CarParkingPlace is null ? DBNull.Value : entity.CarParkingPlace;
            command.Parameters["@PositionBonus"].Value = entity.PositionBonus is null ? DBNull.Value : entity.PositionBonus;

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }

    public override IEnumerable<Position> RunCustomQuery(SqlCommand command)
        => RunCustomQuery(command, ParseToObject);

    private Position ParseToObject(SqlDataReader reader) =>
        new()
        {
            CarParkingPlace = SafeGetString(reader, $"{nameof(Position)}_{nameof(Position.CarParkingPlace)}"),
            PositionBonus = SafeGetDecimal(reader, $"{nameof(Position)}_{nameof(Position.PositionBonus)}"),
            PositionId = GetInt(reader, $"{nameof(Position)}_{nameof(Position.PositionId)}"),
            PositionName = GetString(reader, $"{nameof(Position)}_{nameof(Position.PositionName)}"),
        };

    private string GenerateQuery()
    {
        string commandText = @$"SELECT {string.Join(", ",
            [string.Join(", ", GetDbFields(GlobalConstants.PositionPrefix))])} 
FROM {nameof(Position)}s as {GlobalConstants.PositionPrefix}";

        return commandText;
    }
}
}
