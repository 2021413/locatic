using Locatic.Application.Repositories;
using Locatic.Domain.Entities;
using Locatic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories;

public class ModeleRepository : IModeleRepository
{
    private readonly LocaticDbContext _context;

    public ModeleRepository(LocaticDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Modele>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Modele>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Modele?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Modele>().FindAsync(new object?[] { id }, cancellationToken);

    public async Task AddAsync(Modele entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<Modele>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Modele entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Modele>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Modele entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Modele>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Modele>().FindAsync(new object?[] { id }, cancellationToken) is not null;

    public async Task<IReadOnlyList<Modele>> GetAllWithMarqueAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Modele>().AsNoTracking()
            .Include(m => m.Marque)
            .OrderBy(m => m.Marque!.Nom)
            .ThenBy(m => m.Nom)
            .ToListAsync(cancellationToken);
}
