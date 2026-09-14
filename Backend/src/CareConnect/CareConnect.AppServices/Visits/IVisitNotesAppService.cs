using CareConnect.DTOs.Visits;

namespace CareConnect.AppServices.Visits;

public interface IVisitNotesAppService
{
    Task<IReadOnlyList<VisitNoteDto>> GetVisitNotesAsync(int visitId, CancellationToken cancellationToken);

    Task<int> AddVisitNoteAsync(CreateVisitNoteDto dto, CancellationToken cancellationToken);
}
