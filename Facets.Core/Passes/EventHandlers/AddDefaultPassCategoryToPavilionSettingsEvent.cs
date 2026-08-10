using Facets.Core.Events.Events;
using Facets.Core.Passes.Entities;
using Facets.Core.Passes.Interfaces;
using MediatR;

namespace Facets.Core.Passes.EventHandlers;

internal sealed class AddDefaultPassCategoryToPavilionSettingsEvent : INotificationHandler<PavilionCreatingEvent>
{
    private readonly IPassCategoryRepository _passCategoryRepository;

    public AddDefaultPassCategoryToPavilionSettingsEvent(IPassCategoryRepository passCategoryRepository)
    {
        _passCategoryRepository = passCategoryRepository;
    }

    public async Task Handle(PavilionCreatingEvent notification, CancellationToken cancellationToken)
    {
        var passCategories = await _passCategoryRepository.GetPassCategories(notification.Pavilion.EventId, cancellationToken);

        foreach (var passCategory in passCategories)
        {
            notification.Pavilion.SetPassCategoryPavilionSettings(passCategory.Id, notification.Pavilion.Id, 0.00M);
        }
    }
}
