using System.Data;
using System.Data.SqlClient;
using DatabaseFramework.Models;
using DatabaseFramework.Utilities;

namespace DatabaseFramework.Repos;

public class ProductRepository(string connectionString) : AbstractRepository<Product>(connectionString)
{
    private readonly string _s = connectionString;

    public override Product? GetById(int id)
        => GetById(id, ParseToObject, GenerateQuery);

    public override IEnumerable<Product> FilterFields(WhereParameter param)
        => FilterFields(param, ParseToObject, GenerateQuery);

    public override int AddRow(Product entity)
    {
        using (var connection = new SqlConnection(_s))
        {
            connection.Open();
            var commandText = @$"INSERT INTO {nameof(Product)}s ({string.Join(", ", DbFieldsWithoutPrefix.Where(x=>!x.Equals(IdField)))})
Values (@Name, @Description, @Price, @Supplier) SELECT SCOPE_IDENTITY()";
            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Add("@Name", SqlDbType.VarChar);
            command.Parameters.Add("@Description", SqlDbType.VarChar);
            command.Parameters.Add("@Price", SqlDbType.Decimal);
            command.Parameters.Add("@Supplier", SqlDbType.VarChar);

            command.Parameters["@Name"].Value = entity.ProductName;
            command.Parameters["@Description"].Value = entity.ProductDescription;
            command.Parameters["@Price"].Value = entity.Price;
            command.Parameters["@Supplier"].Value = entity.Supplier;

            
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

    public override bool EditRow(Product entity)
    {
        using (var connection = new SqlConnection(_s))
        {
            connection.Open();
            var commandText = @$"UPDATE {nameof(Product)}s 
SET {nameof(Product.ProductName)} = @Name, {nameof(Product.ProductDescription)} = @Description, {nameof(Product.Price)} = @Price, {nameof(Product.Supplier)} = @Supplier
WHERE {IdField} = @Id";
            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters.Add("@Name", SqlDbType.VarChar);
            command.Parameters.Add("@Description", SqlDbType.VarChar);
            command.Parameters.Add("@Price", SqlDbType.Decimal);
            command.Parameters.Add("@Supplier", SqlDbType.VarChar);

            command.Parameters["@Id"].Value = entity.ProductId;
            command.Parameters["@Name"].Value = entity.ProductName;
            command.Parameters["@Description"].Value = entity.ProductDescription;
            command.Parameters["@Price"].Value = entity.Price;
            command.Parameters["@Supplier"].Value = entity.Supplier;

            var result = command.ExecuteNonQuery();

            return result > 0;
        }
    }

    public override IEnumerable<Product> RunCustomQuery(SqlCommand command)
        => RunCustomQuery(command, ParseToObject);

    private string GenerateQuery()
    {
        string commandText = @$"SELECT {string.Join(", ", GetDbFields(GlobalConstants.ProductsPrefix))} 
FROM {nameof(Product)}s as {GlobalConstants.ProductsPrefix}";
        return commandText;
    }

    private Product ParseToObject(SqlDataReader reader) =>
        new()
        {
            ProductId = GetInt(reader, $"{nameof(Product)}_{nameof(Product.ProductId)}"),
            ProductName = reader.GetString($"{nameof(Product)}_{nameof(Product.ProductName)}"),
            ProductDescription = SafeGetString(reader,$"{nameof(Product)}_{nameof(Product.ProductDescription)}"),
            Price = GetDecimal(reader, $"{nameof(Product)}_{nameof(Product.Price)}"),
            Supplier = reader.GetString($"{nameof(Product)}_{nameof(Product.Supplier)}")
        };
}