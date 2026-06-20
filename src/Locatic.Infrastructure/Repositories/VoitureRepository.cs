using Locatic.Application.Repositories;
using Locatic.Domain.Entities;
using Locatic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories;

public class VoitureRepository : IVoitureRepository
{
    private readonly LocaticDbContext _context;

    public VoitureRepository(LocaticDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Voiture>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Voiture>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Voiture?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Voiture>().FindAsync(new object?[] { id }, cancellationToken);

    public async Task AddAsync(Voiture entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<Voiture>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Voiture entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Voiture>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Voiture entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Voiture>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Voiture>().FindAsync(new object?[] { id }, cancellationToken) is not null;

    public async Task<IReadOnlyList<Voiture>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Voiture>().AsNoTracking()
            .Include(v => v.Modele!)
                .ThenInclude(m => m.Marque)
            .OrderBy(v => v.Modele!.Marque!.Nom)
            .ThenBy(v => v.Modele!.Nom)
            .ToListAsync(cancellationToken);

    public async Task<Voiture?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Voiture>().AsNoTracking()
            .Include(v => v.Modele!)
                .ThenInclude(m => m.Marque)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<bool> ImmatriculationExisteAsync(string immatriculation, int? exclureId = null, CancellationToken cancellationToken = default)
        => await _context.Set<Voiture>().AnyAsync(
            v => v.Immatriculation == immatriculation && (exclureId == null || v.Id != exclureId),
            cancellationToken);
}
