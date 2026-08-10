using Facets.Core.Common.Validators;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;
using Facets.Core.Events.Filters;
using Facets.Core.Events.Interfaces;
using Facets.Core.Events.Specs;
using Facets.Core.Events.Validators;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Events.Services;

public sealed class PavilionService : IPavilionService
{
    private readonly IModelValidator _modelValidator;
    private readonly IPavilionRepository _pavilionRepository;

    public PavilionService(IModelValidator modelValidator, IPavilionRepository pavilionRepository)
    {
        _modelValidator = modelValidator;
        _pavilionRepository = pavilionRepository;
    }

    public async Task<ResponseResult<PavilionDto>> CreatePavilion(Guid eventId, CreatePavilionDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _modelValidator.ValidateAsync<CreatePavilionDtoValidator, CreatePavilionDto>(model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        bool isNameTaken = await _pavilionRepository.IsPavilionNameTaken(eventId, model.Name, cancellationToken);

        if (isNameTaken) return new ResponseResult<PavilionDto>(new OperationFailedException(nameof(model.Name), "Pavilion name is already taken"));

        var pavilion = new Pavilion(eventId, model.Name);

        _pavilionRepository.AddPavilion(pavilion);

        await _pavilionRepository.SaveChangesAsync(cancellationToken);

        return new(new PavilionDto(pavilion.Id, pavilion.Name));
    }

    public async Task<ResponseResult<PavilionSessionDto>> CreatePavilionSession(Guid eventId, Guid pavilionId, CreateOrUpdatePavilionSessionItemDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _modelValidator.ValidateAsync<CreatePavilionSessionDtoValidator, CreateOrUpdatePavilionSessionItemDto>(model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var hasOverlap = await _pavilionRepository.CheckOverlappingSession(eventId, pavilionId, model.EventDateId, model.StartTime, model.EndTime, cancellationToken);

        if (hasOverlap) return new(new OperationFailedException("Overlap", "Pavilion session time is already taken"));

        var pavilion = await _pavilionRepository.GetPavilionBySpec(new GetPavilionToAddSessionsSpec(eventId, pavilionId), cancellationToken, asTracking: true);

        if (pavilion is null) return new(new NotFoundException(nameof(pavilionId), "Pavilion", pavilionId));

        pavilion.SetPavilionSession(model);

        await _pavilionRepository.SaveChangesAsync(cancellationToken);

        var pavilionSessions = pavilion.PavilionSessions.Select(s => new PavilionSessionDto(s.Id,
                                                                                            s.EventDateId,
                                                                                            s.StartTime,
                                                                                            s.EndTime,
                                                                                            s.AllowedVisitorCount,
                                                                                            s.PavilionId))
                                                                                            .FirstOrDefault();

        return new(pavilionSessions);
    }

    public async Task<ResponseResult<PavilionDto>> GetPavilionById(Guid eventId, Guid id, CancellationToken token)
    {
        var pavilionDto = await _pavilionRepository.GetProjectedPavilionBySpec(new PavilionByIdSpec(eventId, id), token);

        if (pavilionDto is null) return new(new NotFoundException(nameof(id), "Pavilion", id));

        return new(pavilionDto);
    }

    public async Task<ResponseResult<IReadOnlyList<PavilionSummaryDto>>> GetPavilions(Guid eventId, Paginator paginator, PavilionFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _pavilionRepository.GetProjectedListBySpec(paginator,
                                                                                    new PavilionListSpec(eventId, filter),
                                                                                    token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult<bool>> CheckPavilionSessionsExist(Guid eventId, CancellationToken token)
    {
        var isPavilionExist = await _pavilionRepository.CheckPavilionSessionsExist(eventId, token);

        return new(isPavilionExist);
    }

    public async Task<ResponseResult<IReadOnlyList<PavilionSessionSummaryDto>>> GetPavilionSessions(Guid eventId, Guid pavilionId, CancellationToken token)
    {
        var (list, totalRecords) = await _pavilionRepository.GetProjectedListBySpec(new Paginator(), new PavilionSessionListSpec(eventId, pavilionId), token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult> UpdatePavilion(Guid eventId, Guid pavilionId, UpdatePavilionDto model, CancellationToken token)
    {
        var validationResult = await _modelValidator.ValidateAsync<UpdatePavilionDtoValidator, UpdatePavilionDto>(model, token);

        if (validationResult.IsValid is false) return new ResponseResult(validationResult.Errors);

        bool isNameTaken = await _pavilionRepository.IsPavilionNameTaken(eventId, model.Name, token, pavilionId);

        if (isNameTaken) return new ResponseResult(new OperationFailedException(nameof(model.Name), "Pavilion name is already taken"));


        var pavilion = await _pavilionRepository.GetPavilionBySpec(new PavilionUpdateSpec(eventId, pavilionId),
                                                                 token,
                                                                 asTracking: true);

        if (pavilion is null) return new ResponseResult(new NotFoundException(nameof(pavilionId), "Pavilion", pavilionId));

        ResponseResult result = pavilion.UpdatePavilionInfo(model.Name);

        if (result.Success is false) return result;

        await _pavilionRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> UpdatePavilionStatus(Guid eventId, Guid pavilionId, UpdatePavilionStatusDto model, CancellationToken token)
    {
        var pavilion = await _pavilionRepository.GetPavilionBySpec(new PavilionUpdateSpec(eventId, pavilionId),
                                                                 token,
                                                                 asTracking: true);

        if (pavilion is null) return new ResponseResult(new NotFoundException(nameof(pavilionId), "Pavilion", pavilionId));

        pavilion.UpdatePavilionStatus(pavilionStatus: model.PavilionStatus);

        await _pavilionRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> UpdatePavilionSession(Guid eventId, Guid pavilionId, Guid pavilionSessionId, CreateOrUpdatePavilionSessionItemDto model, CancellationToken token)
    {
        var validationResult = await _modelValidator.ValidateAsync<UpdatePavilionSessionDtoValidator, CreateOrUpdatePavilionSessionItemDto>(model, token);

        if (validationResult.IsValid is false) return new ResponseResult(validationResult.Errors);

        var hasOverlap = await _pavilionRepository.CheckOverlappingSession(eventId, pavilionId, model.EventDateId, model.StartTime, model.EndTime, token, pavilionSessionId);

        if (hasOverlap) return new ResponseResult(new OperationFailedException("Overlap", "Pavilion session time is already taken"));

        var pavilion = await _pavilionRepository.GetPavilionBySpec(new UpdatePavilionSessionSpec(eventId, pavilionId, pavilionSessionId), token, asTracking: true);

        if (pavilion is null) return new ResponseResult(new NotFoundException(nameof(pavilionId), "Pavilion", pavilionId));

        if (pavilion.PavilionSessions.Count is 0) return new ResponseResult(new NotFoundException(nameof(pavilionSessionId), "Pavilion session", pavilionSessionId));

        var responseResult = pavilion.UpdatePavilionSession(model);

        if (responseResult.Success is false) return responseResult;

        await _pavilionRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> DeletePavilion(Guid eventId, Guid id, CancellationToken token)
    {
        var pavilion = await _pavilionRepository.GetPavilionBySpec(new PavilionDeleteSpec(eventId, id), asTracking: true, token: token);

        if (pavilion is null) return new ResponseResult(new NotFoundException(nameof(id), nameof(Pavilion), id));

        var canDelete = await _pavilionRepository.CanDeletePavilion(eventId, pavilionId: id, token);

        if (canDelete is false) return new(new OperationFailedException("Pavilion", "Cannot delete pavilion"));

        pavilion.Delete();

        await _pavilionRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> DeletePavilionSession(Guid eventId, Guid pavilionSessionId, Guid pavilionId, CancellationToken token)
    {
        var pavilionSession = await _pavilionRepository.GetPavilionSessionBySpec(new PavilionSessionDeleteSpec(eventId, pavilionId, pavilionSessionId), asTracking: true, token: token);

        if (pavilionSession is null) return new ResponseResult(new NotFoundException(nameof(pavilionSessionId), nameof(PavilionSession), pavilionSessionId));

        var canDelete = await _pavilionRepository.CanDeletePavilionSession(eventId, pavilionId: pavilionId, pavilionSessionId: pavilionSessionId,  token);

        if (canDelete is false) return new(new OperationFailedException("Pavilion Session", "Cannot delete pavilion session"));

        pavilionSession.Delete();

        await _pavilionRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }
}