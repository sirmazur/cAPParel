using cAPParel.API.DbContexts;
using cAPParel.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace cAPParel.API.Services.UserServices
{
    public class UserRepository : IUserRepository
    {
        private readonly cAPParelContext _context;
        public UserRepository(cAPParelContext context)
        {
            _context = context;
        }

        public async Task<bool> IsNameAvailable(string name)
        {
            return !(await _context.Users.AnyAsync(u => u.Username == name));
        }

        public async Task<User?> GetUserByName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
        }
    }
}
