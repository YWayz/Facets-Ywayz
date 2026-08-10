using Facets.SharedKernal;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.DTOs;

public sealed record PassTemplateDto(Guid Id,
                                     string TemplateText,
                                     string PreviewTemplateText,
                                     decimal Height,
                                     decimal Width,
                                     AppEnums.PassType PassType,
                                     Guid EventId,
                                     TemplateSizeType SizeType);
