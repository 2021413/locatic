using Locatic.Application.Repositories;
using Locatic.Domain.Entities;
using Locatic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly LocaticDbContext _context;

    public ClientRepository(LocaticDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Client>().AsNoTracking()
            .OrderBy(c => c.Nom)
            .ThenBy(c => c.Prenom)
            .ToListAsync(cancellationToken);

    public async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Client>().FindAsync(new object?[] { id }, cancellationToken);

    public async Task AddAsync(Client entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<Client>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Client entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Client>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Client entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Client>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Client>().FindAsync(new object?[] { id }, cancellationToken) is not null;

    public async Task<bool> EmailExisteAsync(string email, int? exclureId = null, CancellationToken cancellationToken = default)
        => await _context.Set<Client>().AnyAsync(
            c => c.Email == email && (exclureId == null || c.Id != exclureId),
            cancellationToken);
}
