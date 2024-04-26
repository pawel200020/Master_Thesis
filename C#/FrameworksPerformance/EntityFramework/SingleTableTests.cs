using EntityFramework.Models;
using Microsoft.EntityFrameworkCore.Design;

namespace EntityFramework
{
    public class SingleTableTests
    {
        private readonly FrameworkPerformanceMtContext _context;

        public SingleTableTests(FrameworkPerformanceMtContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Run()
        {
            Console.WriteLine(_context.Clients.FirstOrDefault(x => x.ClientId == 12)?.FirstName); 
        }
    }

}
