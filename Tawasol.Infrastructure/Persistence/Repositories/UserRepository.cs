using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.DomainUsers.FindAsync(new object[] { id }, ct);
        }

        public async Task<User?> GetByPhoneAsync(string phone, CancellationToken ct = default)
        {
            return await _context.DomainUsers.FirstOrDefaultAsync(u => u.PhoneNumber == phone, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.DomainUsers.AddAsync(user, ct);
        }

        public Task UpdateAsync(User user)
        {
            _context.DomainUsers.Update(user);
            return Task.CompletedTask;
        }
    }
}
