using cAPParel.API.Entities;

namespace cAPParel.API.Services.VisitServices
{
    public interface IVisitService
    {
        Task<Visit> GetVisit();
        Task AddVisit();
    }
}
