using EntityFrameworkSqlite.Models;

namespace EntityFrameworkSqlite
{
    public class EntityFrameworkSqliteManagerFactory
    {
        public EntityFrameworkSqliteManager Create()
        {
            var context = new FrameworkPerformanceSqlLiteContext();
            return new EntityFrameworkSqliteManager(context);
        }
    }
}
