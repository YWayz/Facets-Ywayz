using Facets.SharedKernal;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.DTOs;

public sealed record CreatePassTemplateDto(string TemplateText,
                                           string PreviewTemplateText,
                                           decimal Height,
                                           decimal Width,
                                           AppEnums.PassType PassType,
                                           TemplateSizeType SizeType);