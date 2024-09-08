using System.Data;
using System.Data.SqlClient;
using DatabaseFramework.Abstraction;
using DatabaseFramework.CustomAttributes;
using DatabaseFramework.Utilities;

namespace DatabaseFramework.Repos
{
    public abstract class AbstractRepository<T>(string connectionString) : IDatabaseEntityRepository<T>
        where T : class
    {
        private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        protected IEnumerable<string> GetDbFields(string prefix) => typeof(T).GetProperties()
            .Where(x => x.CustomAttributes
                .Any(y => y.AttributeType == typeof(DbFieldAttribute)))
            .Select(f => $"{prefix}.{f.Name} as {typeof(T).Name}_{f.Name}")
            .ToList();

        protected readonly IEnumerable<string> DbFieldsWithoutPrefix = typeof(T).GetProperties()
            .Where(x => x.CustomAttributes
                .Any(y => y.AttributeType == typeof(DbFieldAttribute)))
            .Select(f => f.Name)
            .ToList();

        protected readonly string? IdField = typeof(T).GetProperties()
            .Where(x => x.CustomAttributes
                .Any(y => y.AttributeType == typeof(IdAttribute)))
            .Select(x => x.Name)
            .FirstOrDefault();

        protected IEnumerable<string> GetDbFieldsWithoutId<TU>(string prefix) => typeof(TU).GetProperties()
            .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DbFieldAttribute))
                        && x.CustomAttributes.All(y => y.AttributeType != typeof(IdAttribute)))
            .Select(f => $"{prefix}.{f.Name} as {typeof(TU).Name}_{f.Name}").ToList();

        protected IEnumerable<string> GetDbFieldsWithoutIdAndPrefix<TU>() => typeof(TU).GetProperties()
            .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DbFieldAttribute))
                        && x.CustomAttributes.All(y => y.AttributeType != typeof(IdAttribute)))
            .Select(f => f.Name).ToList();

        protected string? GetIdField <TU> () => typeof(TU).GetProperties()
            .Where(x => x.CustomAttributes
                .Any(y => y.AttributeType == typeof(IdAttribute)))
            .Select(f=>f.Name)
            .FirstOrDefault();


        protected string? SafeGetString(SqlDataReader reader, string colName)
            => !reader.IsDBNull(colName) ? reader.GetString(colName) : null;

        protected decimal? SafeGetDecimal(SqlDataReader reader, string colName)
            => !reader.IsDBNull(colName) ? reader.GetDecimal(colName) : null;

        protected int? SafeGetInt(SqlDataReader reader, string colName)
            => !reader.IsDBNull(colName) ? reader.GetInt32(colName) : null;

        protected string? SafeGetStringIdColumn(SqlDataReader reader, int id)
            => !reader.IsDBNull(id) ? reader.GetString(id) : null;

        protected DateTime? SafeGetDate(SqlDataReader reader, string colName)
            => !reader.IsDBNull(colName) ? reader.GetDateTime(reader.GetOrdinal(colName)) : null;

        protected int GetInt(SqlDataReader reader, string colName)
            => reader.GetInt32(reader.GetOrdinal(colName));

        protected string GetString(SqlDataReader reader, string colName)
            => reader.GetString(reader.GetOrdinal(colName));

        protected decimal GetDecimal(SqlDataReader reader, string colName)
            => reader.GetDecimal(reader.GetOrdinal(colName));

        public abstract T? GetById(int id);
        public abstract IEnumerable<T> FilterFields(WhereParameter param);
        public abstract int AddRow(T entity);
        public bool DeleteRow(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                string commandText = $"Delete From {typeof(T).Name}s WHERE {IdField} = @ID";

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = id;

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        public abstract bool EditRow(T entity);
        public int CountRecords(WhereParameter param)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                string commandText = $"SELECT COUNT (*) FROM {typeof(T).Name}s";
                if (!string.IsNullOrEmpty(param.ToString()))
                    commandText = $"SELECT COUNT (*) FROM  {typeof(T).Name}s WHERE {param}";

                SqlCommand command = new SqlCommand(commandText, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }
            }

            return 0;
        }
        public abstract IEnumerable<T> RunCustomQuery(SqlCommand command);
        protected IEnumerable<T> RunCustomQuery(SqlCommand command, Func<SqlDataReader, T> parser)
        {
            List<T> result = new();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                command.Connection = connection;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(parser(reader));
                    }
                }
            }
            return result;
        }
        protected IEnumerable<T> FilterFields(WhereParameter param, Func<SqlDataReader, T> parser, Func<string> queryGenerator)
        {
            var result = new List<T>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                string commandText = queryGenerator();
                if (!string.IsNullOrEmpty(param.ToString()))
                    commandText += $" WHERE {param}";

                SqlCommand command = new SqlCommand(commandText, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(parser(reader));
                    }
                    connection.Close();
                }
            }
            return result;
        }
        protected T? GetById(int id, Func<SqlDataReader, T> parser, Func<string> queryGenerator)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (IdField is null)
                    throw new InvalidOperationException("Id field not detected!");

                string commandText = $"{queryGenerator()} WHERE {IdField} = @Id";

                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = id;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var client = parser(reader);
                        connection.Close();
                        return client;
                    }
                    connection.Close();
                }
            }
            return null;
        }
    }
}
