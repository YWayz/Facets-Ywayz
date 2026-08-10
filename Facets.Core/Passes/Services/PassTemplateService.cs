using Facets.Core.Common.Validators;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Entities;
using Facets.Core.Passes.Filters;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Passes.Specs;
using Facets.Core.Passes.Validators;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace Facets.Core.Passes.Services;

internal sealed class PassTemplateService : IPassTemplateService
{
    private readonly IModelValidator _validator;
    private readonly IPassTemplateRepository _passTemplateRepository;
    private readonly IMemoryCache _memoryCache;

    public PassTemplateService(IModelValidator modelValidator,
                               IPassTemplateRepository passTemplateRepository,
                               IMemoryCache memoryCache)
    {
        _validator = modelValidator;
        _passTemplateRepository = passTemplateRepository;
        _memoryCache = memoryCache;
    }

    public async Task<ResponseResult<PassTemplateDto>> CreatePassTemplate(Guid eventId, CreatePassTemplateDto model, CancellationToken cancellationToken)
    {
        var available = await _passTemplateRepository.IsPassTemplateAvailable(eventId, model.PassType, cancellationToken);

        if (available) return new(new OperationFailedException("Pass Template", "A Pass template already exists"));

        var validationResult = await _validator.ValidateAsync<CreatePassTemplateDtoValidator, CreatePassTemplateDto>(model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        PassTemplate passTemplate = new(eventId,
                                        model.TemplateText,
                                        model.PreviewTemplateText,
                                        model.Height,
                                        model.Width,
                                        model.PassType,
                                        model.SizeType);

        _passTemplateRepository.AddPassTemplate(passTemplate);

        await _passTemplateRepository.SaveChangesAsync(cancellationToken);

        PassTemplateDto template = new(passTemplate.Id,
                                       passTemplate.TemplateText,
                                       passTemplate.PreviewTemplateText,
                                       passTemplate.Height,
                                       passTemplate.Width,
                                       passTemplate.PassType,
                                       passTemplate.EventId,
                                       passTemplate.SizeType);


        _memoryCache.Set($"{model.PassType}:{eventId}", template, TimeSpan.FromDays(3));

        return new(template);
    }


    public async Task<ResponseResult> UpdatePassTemplate(Guid eventId, Guid passTemplateId, UpdatePassTemplateDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<UpdatePassTemplateDtoValidator, UpdatePassTemplateDto>(model, token);

        if (validationResult.IsValid is false) return new ResponseResult(validationResult.Errors);

        var passTemplate = await _passTemplateRepository.GetPassTemplateSpec(new PassTemplateUpdateSpec(eventId, passTemplateId, model.PassType),
                                                                             token,
                                                                             asTracking: true);

        if (passTemplate is null) return new ResponseResult(new NotFoundException(nameof(passTemplateId), "Pass Template", passTemplateId));

        passTemplate.UpdatePassTemplateInfo(model.TemplateText,
                                            model.PreviewTemplateText,
                                            model.Height,
                                            model.Width,
                                            model.SizeType);

        await _passTemplateRepository.SaveChangesAsync(token);

        PassTemplateDto template = new(passTemplate.Id,
                                       passTemplate.TemplateText,
                                       passTemplate.PreviewTemplateText,
                                       passTemplate.Height,
                                       passTemplate.Width,
                                       passTemplate.PassType,
                                       passTemplate.EventId,
                                       passTemplate.SizeType);

        _memoryCache.Set($"{model.PassType}:{eventId}", template, TimeSpan.FromDays(3));

        return new ResponseResult();
    }

    public async Task<ResponseResult<IReadOnlyList<PassTemplateDto>>> GetPassTemplates(Paginator paginator, Guid eventId, PassTemplateFilter filter, CancellationToken token)
    {
        var result = await _memoryCache.GetOrCreateAsync($"{filter.PassType}:{eventId}",
                                                         entry =>
                                                         {
                                                             entry.SetAbsoluteExpiration(TimeSpan.FromDays(3));
                                                             return _passTemplateRepository
                                                                    .GetProjectedPassTemplateSpec(new PassTemplateByIdSpec(eventId, filter),
                                                                                                  token);
                                                         });

        IReadOnlyList<PassTemplateDto> templates = result is null ? [] : [result];

        return new(templates, templates.Count);
    }
}
