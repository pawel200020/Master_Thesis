using DatabaseFramework.Utilities;
using System.Data.SqlClient;

namespace DatabaseFramework.Abstraction
{
    internal interface IDatabaseEntityRepository <T> where T : class
    {
        T? GetById(int id);
        IEnumerable<T> FilterFields(WhereParameter param);
        int AddRow(T entity);
        bool DeleteRow(int id);
        bool EditRow(T entity);
        int CountRecords(WhereParameter param);
        IEnumerable<T> RunCustomQuery(SqlCommand command);
    }
}
