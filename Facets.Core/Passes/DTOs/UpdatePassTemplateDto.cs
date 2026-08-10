using Facets.SharedKernal;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.DTOs;

public sealed record UpdatePassTemplateDto(string TemplateText,
                                           string PreviewTemplateText,
                                           decimal Height,
                                           decimal Width,
                                           TemplateSizeType SizeType,
                                           AppEnums.PassType PassType);
