using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.DTOs;

public sealed record UpdatePaymentSettingsDto(OnSitePayingMode OnSitePayingMode, bool PayLaterForOnlineRegistration);
