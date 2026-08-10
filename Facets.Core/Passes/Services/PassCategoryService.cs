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
namespace Facets.Core.Passes.Services;

internal sealed class PassCategoryService : IPassCategoryService

{
    private readonly IModelValidator _validator;
    private readonly IPassCategoryRepository _passCategoryRepository;

    public PassCategoryService(IModelValidator modelValidator, IPassCategoryRepository passCategoryRepository)
    {
        _validator = modelValidator;
        _passCategoryRepository = passCategoryRepository;
    }

    public async Task<ResponseResult<PassCategoryDto>> CreatePassCategory(Guid eventId, CreatePassCategoryDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<CreatePassCategoryDtoValidator, CreatePassCategoryDto>(model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        bool isNameTaken = await _passCategoryRepository.IsPassCategoryNameTaken(eventId, model.Name);

        if (isNameTaken) return new ResponseResult<PassCategoryDto>(new BadRequestException(nameof(model.Name), "Pass category name is already taken"));

        var passCategory = new Entities.PassCategory(eventId, model.Name, model.Description, model.PassCategoryType, model.Color);

        _passCategoryRepository.AddPassCategory(passCategory);

        await _passCategoryRepository.SaveChangesAsync(cancellationToken);

        return new(new PassCategoryDto(passCategory.Id, passCategory.EventId, passCategory.Name, passCategory.Description, passCategory.VisitorPassCategoryType, passCategory.Color));
    }

    public async Task<ResponseResult<IReadOnlyList<PassCategoryRateDto>>> GetPassCategoryRates(Guid eventId, Paginator paginator, PassCategorySettingsFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _passCategoryRepository.GetProjectedListBySpec(paginator,
                                                                                        new PassCategoryRateListSpec(eventId),
                                                                                        token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult<PassCategoryRateDetailDto>> GetPassCategoryRateById(Guid eventId, Guid passCategoryId, Guid id, CancellationToken token)
    {
        var passCategoryRateDetailDto = await _passCategoryRepository.GetProjectedPassCategoryBySpec(new PassCategoryRateByIdSpec(eventId, passCategoryId, id),
                                                                                                     token);

        if (passCategoryRateDetailDto is null) return new(new NotFoundException(nameof(id), "Pass category rate", id));

        return new(passCategoryRateDetailDto);
    }

    public async Task<ResponseResult> UpdatePassCategoryRate(Guid eventId, Guid passCategoryId, Guid id, UpdatePassCategoryRateDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<UpdatePassCategoryRateDtoValidator, UpdatePassCategoryRateDto>(model, token);

        if (validationResult.IsValid is false) return new ResponseResult(validationResult.Errors);

        var passCategory = await _passCategoryRepository.GetPassCategoryBySpec(new PassCategoryRateUpdateSpec(eventId, passCategoryId, id),
                                                                               token,
                                                                               asTracking: true);

        if (passCategory is null) return new ResponseResult(new NotFoundException(nameof(id), "Pass category rate", id));

        var rateType = passCategory.PassCategorySettings.Select(t => t.RateType).First();

        if (model.RateType != rateType)
        {
            var cannotUpdatePassRateType = await _passCategoryRepository.CanUpdatePassRateType(passCategoryId, eventId, token);

            if (cannotUpdatePassRateType) return new ResponseResult(new OperationFailedException("Pass Category Rate", "Cannot Update Pass Category Rate Type as one or more visitors have been registered"));    
        }


        ResponseResult result = passCategory.SetPassCategory(model.IsChargeable,
                                                             model.Rate,
                                                             model.DiscountedRate,
                                                             model.ApplyEarlyRegistrationDiscountedRate,
                                                             model.ApplyOnlineRegistrationDiscountedRate,
                                                             model.ApplyEntireEventDiscountedRate,
                                                             model.EarlyRegistrationDiscountedRateValidUntil,
                                                             model.RateType);

        if (result.Success is false) return result;

        await _passCategoryRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> UpdatePassCategoryPavilionRate(Guid eventId, Guid pavilionId, UpdatePassCategoryPavilionRateDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<UpdatePassCategoryPavilionRateDtoValidator, UpdatePassCategoryPavilionRateDto>
                                                             (model, token);

        if (validationResult.IsValid is false) return new ResponseResult(validationResult.Errors);

        var passCategories = await _passCategoryRepository.GetPassCategoriesSpec(new PassCategoryPavilionRateUpdateSpec(eventId),
                                                                                 token,
                                                                                 asTracking: true);

        var setPavilionRateResponse = SetPavilionRates(passCategories, model.PavilionRates, pavilionId);

        if (setPavilionRateResponse.Success is false) return setPavilionRateResponse;

        await _passCategoryRepository.SaveChangesAsync(token);

        return new ResponseResult();

        ResponseResult SetPavilionRates(IReadOnlyList<PassCategory> passCategories, IReadOnlyCollection<UpdatePassCategoryPavilionRateItemDto> pavilionRates, Guid pavilionId)
        {
            foreach (var passCategoryPavilionRate in pavilionRates)
            {
                var passCategory = passCategories.FirstOrDefault(w => w.Id == passCategoryPavilionRate.PassCategoryId);

                if (passCategory is null) return new(new NotFoundException(nameof(passCategoryPavilionRate.PassCategoryId), "Pass Category",
                                                                           passCategoryPavilionRate.PassCategoryId));

                passCategory.SetPassCategoryPavilionRate(passCategoryPavilionRate.PassCategoryPavilionSettingsId,
                                                         pavilionId,
                                                         passCategoryPavilionRate.PavilionRate);
            }

            return new();
        }
    }

    public async Task<ResponseResult> UpdateIsChargeableStatus(Guid eventId, Guid passCategoryId, Guid id, UpdatePassCategoryRateIsCharegableStatusDto model, CancellationToken token)
    {
        var passCategory = await _passCategoryRepository.GetPassCategoryBySpec(new PassCategoryRateUpdateIsChargeableStatusSpec(eventId, passCategoryId, id), token, asTracking: true);

        if (passCategory is null) return new ResponseResult(new NotFoundException(nameof(id), "Pass category rate", id));

        passCategory.SetIsChargeableStatus(model.IsChargeable);

        await _passCategoryRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult<PassCategoryDto>> GetPassCategoryById(Guid eventId, Guid id, CancellationToken token)
    {
        var passCategoryDto = await _passCategoryRepository.GetProjectedPassCategorySpec(new PassCategoryByIdSpec(eventId, id), token);

        if (passCategoryDto is null) return new(new NotFoundException(nameof(id), "Pass Category", id));

        return new(passCategoryDto);
    }

    public async Task<ResponseResult<PassCategoryDto>> GetTeamMemberPassCategoryById(Guid eventId, Guid id, CancellationToken token)
    {
        var passCategoryDto = await _passCategoryRepository.GetProjectedPassCategorySpec(new TeamMemberPassCategoryByIdSpec(eventId, id), token);

        if (passCategoryDto is null) return new(new NotFoundException(nameof(id), "Pass Category", id));

        return new(passCategoryDto);
    }

    public async Task<ResponseResult<IReadOnlyList<PassCategoryDto>>> GetPassCategories(Guid eventId, Paginator paginator, PassCategoryFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _passCategoryRepository.GetProjectedListBySpec(paginator,
                                                                                               new PassCategoryListSpec(eventId, filter),
                                                                                               token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult> UpdatePassCategory(Guid eventId, Guid passCategoryId, UpdatePassCategoryDto model, CancellationToken token)
    {
        bool isNameTaken = await _passCategoryRepository.IsPassCategoryNameTaken(eventId, model.Name, passCategoryId, token);

        if (isNameTaken) return new ResponseResult(new BadRequestException(nameof(model.Name), "Pass category name is already taken"));

        var validationResult = await _validator.ValidateAsync<UpdatePassCategoryDtoValidator, UpdatePassCategoryDto>(model, token);

        if (validationResult.IsValid is false) return new ResponseResult(validationResult.Errors);

        var passCategory = await _passCategoryRepository.GetPassCategorySpec(new PassCategoryUpdateSpec(eventId, passCategoryId),
                                                                             token, asTracking: true);

        if (passCategory is null) return new ResponseResult(new NotFoundException(nameof(passCategoryId), "Pass Category", passCategoryId));

        ResponseResult result = passCategory.UpdatePassCategoryInfo(model.Name, model.Description, model.Color);

        if (result.Success is false) return result;

        await _passCategoryRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> UpdateVisitorPassCategoryType(Guid eventId, Guid passCategoryId, UpdateVisitorPassCategoryTypeDto model, CancellationToken token)
    {
        var passCategory = await _passCategoryRepository.GetPassCategorySpec(new PassCategoryUpdateSpec(eventId, passCategoryId),
                                                                             token, asTracking: true);

        if (passCategory is null) return new ResponseResult(new NotFoundException(nameof(passCategoryId), "Pass Category", passCategoryId));

        ResponseResult result = passCategory.UpdateVisitorPassCategoryTypeInfo(model.VisitorPassCategoryType);

        if (result.Success is false) return result;

        await _passCategoryRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> Delete(Guid eventId, Guid id, CancellationToken token)
    {
        var passCategory = await _passCategoryRepository.GetPassCategorySpec(new PassCategoryDeleteSpec(eventId, id), asTracking: true, token: token);

        if (passCategory is null) return new ResponseResult(new NotFoundException(nameof(id), nameof(Entities.PassCategory), id));

        var canDeleteResponseResult = await _passCategoryRepository.CanDeletePassCategory(id, token);

        if (canDeleteResponseResult.Success is false) return canDeleteResponseResult;

        var canDeleteResponse = passCategory.Delete();

        if (canDeleteResponse.Success is false) return canDeleteResponse;

        await _passCategoryRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult<PassCategoryDto>> GetPassCategoryForActiveEventById(Guid eventId, Guid id, CancellationToken token)
    {
        var passCategoryDto = await _passCategoryRepository.GetProjectedPassCategorySpec(new PassCategoryForActiveEventByIdSpec(eventId, id), token);

        if (passCategoryDto is null) return new(new NotFoundException(nameof(id), "Pass Category", id));

        return new(passCategoryDto);
    }
}
