using Facets.Core.Events.DTOs;
using Facets.Core.Events.Events;
using Facets.Core.Passes.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Entities;

public sealed class Pavilion : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public string Name { get; private set; }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;

    private readonly List<PavilionSession> _pavilionSessions = new();
    public IReadOnlyCollection<PavilionSession> PavilionSessions => _pavilionSessions.AsReadOnly();

    private readonly List<PassCategoryPavilionSettings> _passCategoryPavilionSettings = new();
    public IReadOnlyCollection<PassCategoryPavilionSettings> PassCategoryPavilionSettings => _passCategoryPavilionSettings.AsReadOnly();

    public PavilionStatus Status { get; private set; } = PavilionStatus.Active;

    private Pavilion() { }

    public Pavilion(Guid eventId, string name)
    {
        EventId = eventId;
        Name = name;

        RegisterDomainEvent(new PavilionCreatingEvent(isPrePersistantDomainEvent: true, this));
    }

    internal void SetPavilionSession(CreateOrUpdatePavilionSessionItemDto pavilionSession)
    {
        _pavilionSessions.Add(new PavilionSession(pavilionSession.EventDateId,
                                                  pavilionSession.StartTime,
                                                  pavilionSession.EndTime,
                                                  pavilionSession.AllowedVisitorCount));
    }

    internal void SetPassCategoryPavilionSettings(Guid passCategoryId, Guid pavilionId, decimal pavilionRate)
    {
        _passCategoryPavilionSettings.Add(new PassCategoryPavilionSettings(passCategoryId,
                                                                           pavilionId,
                                                                           pavilionRate));
    }

    internal ResponseResult UpdatePavilionSession(CreateOrUpdatePavilionSessionItemDto model)
    {
        if (PavilionSessions.First()!.VisitorPavilionSessionAttendanceSchedules.Count > 0 &&
        model.StartTime.TimeOfDay != PavilionSessions.First()!.StartTime.TimeOfDay &&
                                          model.EndTime.TimeOfDay != PavilionSessions.First()!.EndTime.TimeOfDay)
            return new ResponseResult(new OperationFailedException("Pavilion session", "Visitor is already registered to the pavilion sessions"));

        if (model.AllowedVisitorCount < PavilionSessions.First()!.VisitorPavilionSessionAttendanceSchedules.Count)
            return new ResponseResult(new OperationFailedException("Pavilion session", "Visitor registration count exceeds the visitor allowed count"));

        _pavilionSessions.First().Update(model.StartTime,
                                         model.EndTime,
                                         model.AllowedVisitorCount);

        return new();
    }

    internal void Delete() => IsDeleted = true;

    internal ResponseResult UpdatePavilionInfo(string name)
    {
        Name = name;

        return new();
    }

    internal void UpdatePavilionStatus(PavilionStatus pavilionStatus)
    {
        Status = pavilionStatus;
    }
}
