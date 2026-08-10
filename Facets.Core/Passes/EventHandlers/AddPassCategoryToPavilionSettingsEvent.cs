using Facets.Core.Events.Interfaces;
using Facets.Core.Passes.Entities;
using Facets.Core.Passes.Events;
using Facets.Core.Passes.Interfaces;
using MediatR;

namespace Facets.Core.Passes.EventHandlers;

internal sealed class AddPassCategoryToPavilionSettingsEvent : INotificationHandler<PassCategoryCreatingEvent>
{
    private readonly IPavilionRepository _pavilionRepository;

    public AddPassCategoryToPavilionSettingsEvent(IPavilionRepository pavilionRepository)
    {
        _pavilionRepository = pavilionRepository;
    }

    public async Task Handle(PassCategoryCreatingEvent notification, CancellationToken cancellationToken)
    {
        var pavilions = await _pavilionRepository.GetPavilions(notification.PassCategory.EventId, cancellationToken);

        foreach (var pavilion in pavilions)
        {
            notification.PassCategory.SetPassCategoryPavilionSettings(notification.PassCategory.Id, pavilion.Id, 0.00M);
        }
    }
}
