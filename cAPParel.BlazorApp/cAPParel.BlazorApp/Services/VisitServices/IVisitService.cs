using cAPParel.BlazorApp.Models;

namespace cAPParel.BlazorApp.Services.VisitServices
{
    public interface IVisitService
    {
        Task<Visit> GetVisitAsync();
        Task AddVisitAsync();
    }
}
