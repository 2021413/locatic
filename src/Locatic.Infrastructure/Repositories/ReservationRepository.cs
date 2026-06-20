using Locatic.Application.Repositories;
using Locatic.Domain.Entities;
using Locatic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly LocaticDbContext _context;

    public ReservationRepository(LocaticDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Reservation>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Reservation>().FindAsync(new object?[] { id }, cancellationToken);

    public async Task AddAsync(Reservation entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<Reservation>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Reservation entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Reservation>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Reservation entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Reservation>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Reservation>().FindAsync(new object?[] { id }, cancellationToken) is not null;

    public async Task<IReadOnlyList<Reservation>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Reservation>().AsNoTracking()
            .Include(r => r.Client)
            .Include(r => r.Voiture!)
                .ThenInclude(v => v.Modele!)
                    .ThenInclude(m => m.Marque)
            .OrderByDescending(r => r.DateDebut)
            .ToListAsync(cancellationToken);

    public async Task<bool> ChevauchementExisteAsync(
        int voitureId,
        DateOnly debut,
        DateOnly fin,
        int? exclureReservationId = null,
        CancellationToken cancellationToken = default)
        => await _context.Set<Reservation>().AnyAsync(
            r => r.VoitureId == voitureId
                 && (exclureReservationId == null || r.Id != exclureReservationId)
                 // Deux périodes se chevauchent si chacune commence avant la fin de l'autre.
                 && debut < r.DateFin
                 && r.DateDebut < fin,
            cancellationToken);

    public async Task<int> CompterEnCoursAsync(DateOnly date, CancellationToken cancellationToken = default)
        => await _context.Set<Reservation>().CountAsync(r => r.DateDebut <= date && date < r.DateFin, cancellationToken);

    public async Task<bool> VoitureADesReservationsAsync(int voitureId, CancellationToken cancellationToken = default)
        => await _context.Set<Reservation>().AnyAsync(r => r.VoitureId == voitureId, cancellationToken);
}
