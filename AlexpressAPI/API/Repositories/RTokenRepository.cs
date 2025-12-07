using API.DbContexts;
using API.Entities;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class RTokenRepository : IRTokenRepository
    {
        private readonly RTokenDbContext _context;
        public RTokenRepository(RTokenDbContext c)
        {
            _context = c;
        }
        public async Task<RToken?> GetRefreshTokenAsync(string refreshToken)
        {
            var rToken = await _context.RTokens.FirstOrDefaultAsync(r => r.RefreshToken == refreshToken);

            return rToken;
        }

        public async Task RevokeRTokenAsync(RToken r)
        {
            r.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        public async Task<RToken> CreateRefreshToken(RToken r)
        {
            _context.RTokens.Add(r);

            await _context.SaveChangesAsync();

            return r;
        }
    }
}
