using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.DTOs;

public sealed record PaymentSettingsDto(Guid EventId, OnSitePayingMode OnSitePayingMode, bool PayLaterForOnlineRegistration);
