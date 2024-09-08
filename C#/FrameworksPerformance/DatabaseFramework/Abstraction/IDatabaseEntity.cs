namespace DatabaseFramework.Abstraction
{
    public interface IDatabaseEntity
    {
        static string? TableName { get; }
    }
}
