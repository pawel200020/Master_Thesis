using EntityFramework.Models;

namespace EntityFramework
{
    public class EntityFrameworkManagerFactory
    {
        public EntityFrameworkManager Create()
        {
            var context = new FrameworkPerformanceMtContext();
            return new EntityFrameworkManager(context);
        }
    }
}
