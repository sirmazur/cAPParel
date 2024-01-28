using cAPParel.API.DbContexts;
using cAPParel.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace cAPParel.API.Services.VisitServices
{
    public class VisitService : IVisitService
    {
        private readonly cAPParelContext _context;
        public VisitService(cAPParelContext context) 
        {
            _context = context;
        }

        public async Task<Visit> GetVisit()
        {
            return await _context.Visits.FirstOrDefaultAsync();
        }

        public async Task AddVisit()
        {
            var visit = await _context.Visits.FirstOrDefaultAsync();
            visit.Amount++;
            await _context.SaveChangesAsync();
        }
    }
}

