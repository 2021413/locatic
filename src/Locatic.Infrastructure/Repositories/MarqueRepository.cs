using Locatic.Application.Repositories;
using Locatic.Domain.Entities;
using Locatic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories;

public class MarqueRepository : IMarqueRepository
{
    private readonly LocaticDbContext _context;

    public MarqueRepository(LocaticDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Marque>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Marque>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Marque?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Marque>().FindAsync(new object?[] { id }, cancellationToken);

    public async Task AddAsync(Marque entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<Marque>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Marque entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Marque>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Marque entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Marque>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Marque>().FindAsync(new object?[] { id }, cancellationToken) is not null;

    public async Task<IReadOnlyList<Marque>> GetAllWithModelesAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Marque>().AsNoTracking()
            .Include(m => m.Modeles)
            .OrderBy(m => m.Nom)
            .ToListAsync(cancellationToken);

    public async Task<bool> NomExisteAsync(string nom, CancellationToken cancellationToken = default)
        => await _context.Set<Marque>().AnyAsync(m => m.Nom == nom, cancellationToken);
}
