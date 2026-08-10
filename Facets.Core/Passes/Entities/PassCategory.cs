using Facets.Core.Events.Entities;
using Facets.Core.Passes.Events;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.Entities;

public sealed class PassCategory : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    private static PassCategorySettings _defualtPassCategorySetting => new(isChargeable: false,
                                                                           rate: 0,
                                                                           discountedRate: 0,
                                                                           applyEarlyRegistrationDiscountedRate: false,
                                                                           applyOnlineRegistrationDiscountedRate: false,
                                                                           applyEntireEventDiscountedRate: false,
                                                                           earlyRegistrationDiscountedRateValidUntil: null);

    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public Guid PassTypeId { get; private set; }
    public PassType? PassType { get; private set; }
    public VisitorPassCategoryType VisitorPassCategoryType { get; private set; }
    public bool IsDefault { get; private set; }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;

    public PassCategoryType PassCategoryType { get; set; }

    private readonly List<PassCategorySettings> _passCategorySettings = new();

    public IReadOnlyCollection<PassCategorySettings> PassCategorySettings => _passCategorySettings.AsReadOnly();

    private readonly List<PassCategoryPavilionSettings> _passCategoryPavilionSettings = new();

    public IReadOnlyCollection<PassCategoryPavilionSettings> PassCategoryPavilionSettings => _passCategoryPavilionSettings.AsReadOnly();
    public string Color { get; private set; } = null!;

    public static IReadOnlyList<PassCategory> GetDefaultData(Guid eventId)
    {
        IReadOnlyList<PassCategory> defaultPassTypes = new[]
        {
           new PassCategory()
           {
              Name = "Member",
              Description="Association Member",
              PassTypeId = PassType.VisitorPassTypeId,
              EventId = eventId,
              IsDefault = true,
              PassCategoryType= PassCategoryType.AssocifyMember,
           },

           new PassCategory ()
           {
              Name = "Foreign - Buyer",
              Description="Foreigner & Buyer",
              PassTypeId = PassType.VisitorPassTypeId,
              EventId = eventId,
              IsDefault = true,
              PassCategoryType= PassCategoryType.ForeignBuyer,
           },

           new PassCategory ()
           {
              Name = "Visitor",
              Description="Local and Foreign visitors",
              PassTypeId = PassType.VisitorPassTypeId,
              EventId = eventId,
              IsDefault = true,
              PassCategoryType= PassCategoryType.LocalAndForeignVisitor,
           }
        };

        foreach (var defaultPassType in defaultPassTypes) defaultPassType._passCategorySettings.Add(_defualtPassCategorySetting);

        return defaultPassTypes;
    }

    private PassCategory() { }

    public PassCategory(Guid eventId, string name, string? description, PassCategoryType passCategoryType, string color)
    {
        EventId = eventId;
        Name = name;
        Description = description;
        PassTypeId = passCategoryType is PassCategoryType.TeamMember ? PassType.TeamMemberPassTypeId : PassType.VisitorPassTypeId;
        _passCategorySettings.Add(_defualtPassCategorySetting);
        PassCategoryType = passCategoryType;
        Color = color;

        RegisterDomainEvent(new PassCategoryCreatingEvent(isPrePersistantDomainEvent: true, this));
    }

    public ResponseResult SetPassCategory(bool isChargeable,
                                         decimal rate,
                                         decimal discountedRate,
                                         bool applyEarlyRegistrationDiscountedRate,
                                         bool applyOnlineRegistrationDiscountedRate,
                                         bool applyEntireEventDiscountedRate,
                                         DateTimeOffset? earlyRegistrationDiscountedRateValidUntil,
                                         RateType rateType)
    {
        PassCategorySettings.First().UpdatePassCategoryRateInfo(isChargeable,
                                                                rate,
                                                                discountedRate,
                                                                applyEarlyRegistrationDiscountedRate,
                                                                applyOnlineRegistrationDiscountedRate,
                                                                applyEntireEventDiscountedRate,
                                                                earlyRegistrationDiscountedRateValidUntil,
                                                                rateType);
        return new ResponseResult();
    }

    internal void SetPassCategoryPavilionSettings(Guid passCategoryId, Guid pavilionId, decimal pavilionRate)
    {
        _passCategoryPavilionSettings.Add(new PassCategoryPavilionSettings(passCategoryId,
                                                                           pavilionId,
                                                                           pavilionRate));
    }
    public void SetPassCategoryPavilionRate(Guid passCategoryPavilionSettingsId, Guid pavilionId, decimal pavilionRate)
    {
        PassCategoryPavilionSettings.First(f => f.Id == passCategoryPavilionSettingsId && f.PavilionId == pavilionId).UpdatePavilionRate(pavilionRate);
    }

    internal ResponseResult SetIsChargeableStatus(bool isChargeable)
    {
        PassCategorySettings.First().UpdateIsChargeableStatus(isChargeable);
        return new ResponseResult();
    }

    internal ResponseResult Delete()
    {
        if (IsDefault is true) return new(new OperationFailedException("Pass category", "Cannot delete default pass category"));

        IsDeleted = true;
        return new();
    }

    internal ResponseResult UpdatePassCategoryInfo(string name, string? description, string color)
    {
        Name = name;
        Description = description;
        Color = color;

        return new();
    }

    internal ResponseResult UpdateVisitorPassCategoryTypeInfo(VisitorPassCategoryType visitorPassCategoryType)
    {
        VisitorPassCategoryType = visitorPassCategoryType;

        return new();
    }
}
