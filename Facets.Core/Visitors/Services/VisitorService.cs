using Facets.Core.Common.Dtos;
using Facets.Core.Common.Interfaces;
using Facets.Core.Common.Validators;
using Facets.Core.Reports.Dtos;
using Facets.Core.Reports.Filters;
using Facets.Core.Reports.Interfaces;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Filters;
using Facets.Core.Visitors.Interfaces;
using Facets.Core.Visitors.Specs;
using Facets.Core.Visitors.Validators;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Services;

internal sealed class VisitorService : IVisitorService
{
    private readonly IModelValidator _validator;
    private readonly IVisitorActivityService _visitorActivityService;
    private readonly IVisitorRepository _visitorRepository;
    private readonly IFileRespository _fileRespository;
    private readonly ILoggedInUserService _loggedInUser;

    public VisitorService(IModelValidator validator, IVisitorActivityService visitorActivityService, IVisitorRepository visitorRepository, IFileRespository fileRespository, ILoggedInUserService loggedInUser)
    {
        _validator = validator;
        _visitorActivityService = visitorActivityService;
        _visitorRepository = visitorRepository;
        _fileRespository = fileRespository;
        _loggedInUser = loggedInUser;
    }

    public async Task<ResponseResult<VisitorCreatedDto>> RegisterVisitor(RegisterVisitorInternalDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<RegisterVisitorInternalDtoValidator, RegisterVisitorInternalDto>
                                                             (model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        Visitor visitor = new(model.VisitorIdentityType,
                              model.NICNumber,
                              model.PassportNumber,
                              model.CountryId,
                              model.FirstName,
                              model.LastName,
                              model.MobileNumber,
                              model.Email,
                              model.CompanyName,
                              model.Address,
                              isAssocifyMember: model.IsAssocifyMember,
                              registeredOnline: model.RegisterOnline);

        _visitorRepository.Add(visitor);

        await _visitorRepository.SaveChangesAsync(cancellationToken);

        return new(new VisitorCreatedDto(visitor.Id,
                                         visitor.IsAssocifyMember,
                                         visitor.VisitorIdentityType,
                                         visitor.NICNumber,
                                         visitor.PassportNumber,
                                         visitor.CountryId,
                                         visitor.RegisteredOnline,
                                         visitor.OTPVerified,
                                         visitor.OTPVerificationRequired,
                                         visitor.FirstName,
                                         visitor.LastName,
                                         visitor.MobileNumber,
                                         visitor.CompanyName,
                                         visitor.Email,
                                         visitor.Address,
                                         visitor.VisitorStatus));
    }

    public async Task<ResponseResult<VisitorSearchDto>> SearchVisitor(string? searchValue, CancellationToken token)
    {
        var visitor = await _visitorRepository.GetProjectedVisitorBySpec(new VisitorSearchSpec(searchValue), token);

        return new(visitor);
    }

    public async Task<ResponseResult<IReadOnlyList<FileDto>>> AddDocumentsToExistingVisitor(Guid visitorId, List<KeyValuePair<AttachmentType, IFormFile>> files, CancellationToken token)
    {
        if (files.Select(s => s.Key).Any(a => a is AttachmentType.None))
            return new(new BadRequestException(nameof(AttachmentType), $"One or more document type is having the Key {nameof(AttachmentType.None)}"));

        var docs = files.Select(s => s.Value).ToList();

        var validationResult = await _validator.ValidateAsync<ImageFileCollectionValidator, IEnumerable<IFormFile>>(docs, token);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var visitor = await _visitorRepository.GetVisitorBySpec(new VisitorImageUploadSpec(visitorId), token, asTracking: true);

        if (visitor is null) return new(new NotFoundException(nameof(visitorId), "Visitor", visitorId));

        List<FileDto> fileListDto = new();

        if (files is null or { Count: 0 }) return new(fileListDto);

        foreach (var file in files)
        {
            var doc = file.Value;
            using var stream = doc.OpenReadStream();

            var compressedStream = ImageHelper.CompressImage(stream);

            var documentUploadResponse = await _fileRespository.UploadFile(compressedStream,
                                                                           documentName: doc.FileName,
                                                                           contentType: doc.ContentType,
                                                                           containerName: AppConstants.BlobStorage.ContainerName.VisitorDocuments,
                                                                           folderPath: visitorId.ToString(),
                                                                           cancellationToken: token);

            if (documentUploadResponse.Success is false) return new(new BadRequestException("FileUpload", $"File ({doc.FileName}) failed to upload"));

            var uploadedFileInfo = documentUploadResponse.Data!;

            FileDto documentDto = new()
            {
                Id = uploadedFileInfo.URI,
                FileName = uploadedFileInfo.FileName,
                UniqueName = uploadedFileInfo.UniqueName,
                URI = uploadedFileInfo.URI,
                RelatedEntityId = visitorId,
                ExtenstionData = new { AttachmentType = file.Key }
            };

            fileListDto.Add(documentDto);

            visitor.AddDocument(uploadedFileInfo.URI, file.Key, uniqueName: documentDto.UniqueName, displayName: documentDto.FileName);
        }

        await _visitorRepository.SaveChangesAsync(token);

        return new(fileListDto);
    }

    public async Task<ResponseResult> UpdateVisitor(Guid visitorId, UpdateVisitorDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<UpdateVisitorOnsiteDtoValidator, UpdateVisitorDto>(model, token);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var visitor = await _visitorRepository.GetVisitorBySpec(new VisitorUpdateSpec(visitorId), token, asTracking: true);

        if (visitor is null) return new ResponseResult(new NotFoundException(nameof(visitorId), "Visitor", visitorId));

        visitor.UpdateInfo(model.VisitorIdentityType,
                           model.NICNumber,
                           model.PassportNumber,
                           model.CountryId,
                           model.FirstName,
                           model.LastName,
                           model.MobileNumber,
                           model.Email,
                           model.CompanyName,
                           model.Address);

        await _visitorRepository.SaveChangesAsync(token);

        return new();
    }

    public async Task<ResponseResult<VisitorDto>> GetVisitorById(Guid id, CancellationToken token)
    {
        var visitor = await _visitorRepository.GetProjectedVisitorBySpec(new VisitorByIdSpec(id), token);

        if (visitor is null) return new(new NotFoundException(nameof(id), "Visitor", id));

        return new(visitor);
    }

    public async Task<ResponseResult<IReadOnlyList<VisitorSummaryDto>>> GetVisitors(Paginator paginator, VisitorFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _visitorRepository.GetProjectedListBySpec(paginator, new VisitorListSpec(filter), token);

        return new(list, totalRecords);
    }

    public void AddVisitorActivity(VisitorActivity visitorActivity)
    {
        _visitorRepository.AddVisitorActivity(visitorActivity);
    }

    public async Task<ResponseResult<IReadOnlyList<FileDto>>> GetDocuments(Guid visitorId, VisitorDocumentFilter filter, CancellationToken token)
    {
        var visitor = await _visitorRepository.GetVisitorBySpec(new VisitorWithDocumentSpec(visitorId, filter), token, asTracking: true);

        if (visitor is null) return new(new NotFoundException(nameof(visitorId), "Visitor", visitorId));

        return new(visitor.Documents.Select(s => new FileDto
        {
            Id = s.Id.ToString(),
            ExtenstionData = new { s.AttachmentType, s.CreatedOn },
            FileName = s.DisplayName,
            RelatedEntityId = s.VisitorId,
            UniqueName = s.UniqueName,
            URI = s.AttachmentURL
        }).ToList().AsReadOnly());
    }
    public async Task<ResponseResult> DeleteDocument(Guid visitorId, Guid id, CancellationToken token)
    {
        var visitor = await _visitorRepository.GetVisitorBySpec(new VisitorDocumentDeleteSpec(visitorId, id), token, asTracking: true);

        if (visitor is null) return new(new NotFoundException(nameof(visitorId), "Visitor", visitorId));

        if (visitor.Documents.Any() is false) return new ResponseResult(new NotFoundException(nameof(id), "Document", id));

        visitor.DeleteDocument(id);

        await _visitorRepository.SaveChangesAsync(token);

        return new();
    }

    public async Task<ResponseResult> MarkAsBlackListed(Guid visitorId, MarkAsBlackListedDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<MarkAsBlackListedDtoValidator, MarkAsBlackListedDto>(model, token);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var visitor = await _visitorRepository.GetVisitorBySpec(new VisitorBlacklistSpec(visitorId), token, asTracking: true);

        if (visitor is null) return new(new NotFoundException(nameof(visitorId), "Visitor", visitorId));

        if (visitor.VisitorStatus is VisitorStatus.BlackListed) return new();

        visitor.MarkAsBlacklisted(model.BlackListUntil, model.Reason);


        var blacklistedDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");
        var blacklistedUntil = model.BlackListUntil?.GetLocalTime(AppConstants.SriLankaTimeZone).ToApplicationDateFormat();

        var description = $"{blacklistedDateAndTime} - Blacklisted {(blacklistedUntil is not null ? " until " + blacklistedUntil : string.Empty)}";

        VisitorActivity activity = new(_loggedInUser.FacetsEventId,
                                       visitor.Id,
                                       description,
                                       VisitorActivityType.MarkedAsBlacklisted);

        _visitorRepository.AddVisitorActivity(activity);

        await _visitorRepository.SaveChangesAsync(token);

        return new();
    }

    public async Task<ResponseResult> RemoveFromBlackList(Guid visitorId, CancellationToken token)
    {
        var visitor = await _visitorRepository.GetVisitorBySpec(new VisitorBlacklistSpec(visitorId), token, asTracking: true);

        if (visitor is null) return new(new NotFoundException(nameof(visitorId), "Visitor", visitorId));

        if (visitor.VisitorStatus is VisitorStatus.Active) return new();

        visitor.RemoveFromBlacklist();

        var unblacklistedDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");

        var description = $"{unblacklistedDateAndTime} - Removed from the blacklist";

        VisitorActivity activity = new(_loggedInUser.FacetsEventId,
                                       visitor.Id,
                                       description,
                                       VisitorActivityType.RemovedFromBlackList);

        _visitorRepository.AddVisitorActivity(activity);

        await _visitorRepository.SaveChangesAsync(token);

        return new();
    }

    public async Task<ResponseResult<VisitorDto>> GetVisitorByIdentificationNumber(string identityNumber, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetProjectedVisitorBySpec(new VisitorByIdentificationNumberSpec(identityNumber), cancellationToken);

        if (visitor is null) return new(new NotFoundException(nameof(identityNumber), "Visitor", identityNumber));

        return new(visitor);
    }

    public async Task<ResponseResult> UpdateOTPVerificationStatus(string identityNumber, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetVisitorBySpec(new PublicVisitorOTPVerificationSpec(identityNumber),
                                                                cancellationToken,
                                                                asTracking: true);

        if (visitor is null) return new(new OperationFailedException("Visitor OTP verfication", "Visitor not found"));

        var responseResult = visitor.UpdateOTPVerificationStatus();

        if (responseResult.Success is false) return responseResult;

        return new();
    }

    public async Task<ResponseResult<IReadOnlyList<VisitorActivityDto>>> GetVisitorActivities(Paginator paginator, Guid visitorId, CancellationToken token)
    {
        var visitorActivities = await _visitorActivityService.GetVisitorActivities(paginator, visitorId, _loggedInUser.FacetsEventId, token);

        return visitorActivities;
    }


    public async Task<ResponseResult<IReadOnlyList<AttendenceReportDto>>> AttendanceReport(Paginator paginator, AttendancereportFilter filter, CancellationToken token)
    {
        var visitorReportDetails = await _visitorRepository.AttendanceCollectionReport(paginator, filter, token);
        return visitorReportDetails;
    }


}




